'use client';

import React, { useState, useEffect, useRef } from 'react';
import { useParams } from 'next/navigation';
import { Layout, Row, Col, Button, Typography, theme, Spin, Empty, List, message as messageApi, Tooltip } from 'antd';
import { FileTextOutlined, GithubFilled, CopyOutlined, FileOutlined, FileMarkdownOutlined, FileImageOutlined, FileExcelOutlined, FileWordOutlined, FilePdfOutlined, FileUnknownOutlined, CodeOutlined } from '@ant-design/icons';
import { getChatShareMessageList } from '../../services/chatShareMessageServce';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { homepage } from '../../const/urlconst';
import { API_URL, fetchSSE, getFileContent } from '../../services';
import { DocumentContent } from '../../components/document';

const { Text } = Typography;
const { useToken } = theme;

// 定义消息类型
interface ChatMessage {
  content: string;
  sender: 'user' | 'ai';
  loading?: boolean;
}


export default function SearchPage({ }: any) {
  const { token } = useToken();
  const params = useParams();
  const chatShareMessageId = params.query as string;
  // 添加消息容器引用
  const messagesContainerRef = useRef<HTMLDivElement>(null);

  const [message, setMessage] = useState('');
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [loading, setLoading] = useState(false);
  const [warehouseId, setWarehouseId] = useState('')

  // 模拟引用文件列表
  const [referenceFiles, setReferenceFiles] = useState<Array<{
    path: string;
    title: string;
    content?: string;
  }>>([]);
  const [fileListLoading, setFileListLoading] = useState(false);
  const [selectedFile, setSelectedFile] = useState<string | null>(null);
  const [fileContent, setFileContent] = useState('');
  const [fileContentLoading, setFileContentLoading] = useState(false);

  // 初始化页面时，如果有初始消息，自动发送
  useEffect(() => {
    if (chatShareMessageId) {
      loadInitMessage();
    }
  }, [chatShareMessageId]);

  const loadInitMessage = async () => {
    const { data } = await getChatShareMessageList(chatShareMessageId, 1, 10);
    console.log(data.data.items);

    if (data.data.items.length === 0) {
      if (data.data.info && data.data.info.question) {
        messages.push({
          content: data.data.info.question,
          sender: 'user'
        })
        setWarehouseId(data.data.info.warehouseId)
        setMessages([...messages]);
      }
      handleSendMessage('', true);
    } else {
      // 循环处理消息列表
      const messageList = data.data.items.sort((a: any, b: any) => a.id - b.id);
      const newMessages: ChatMessage[] = [];

      messageList.forEach((item: any) => {
        newMessages.push({
          content: item.question || '',
          sender: 'user'
        });

        if (item.answer) {
          newMessages.push({
            content: item.answer,
            sender: 'ai'
          });
        }
      });

      // 获取最后一条消息的引用文件并渲染
      const lastMessage = messageList[messageList.length - 1];
      if (lastMessage && lastMessage.files && lastMessage.files.length > 0) {
        const files = lastMessage.files.map((x: string) => {
          const value = x.split('/');
          const title = value[value.length - 1];
          return {
            path: x,
            title,
          }
        });
        setReferenceFiles(files);
      }

      if (data.data.info && data.data.info.warehouseId) {
        setWarehouseId(data.data.info.warehouseId);
      }

      setMessages(newMessages);
    }
  }

  // 渲染消息气泡
  const renderMessage = (msg: ChatMessage, index: number) => {
    const isUser = msg.sender === 'user';

    return (
      <div
        key={index}
        style={{
          display: 'flex',
          flexDirection: 'row',
          justifyContent: isUser ? 'flex-end' : 'flex-start',
        }}
      >
        <div
          style={{
            maxWidth: '80%',
            padding: token.paddingSM,
            borderRadius: token.borderRadiusLG,
            backgroundColor: token.colorBgElevated,
            color: token.colorText,
            fontSize: token.fontSize,
          }}
        >
          {msg.loading ? (
            <Spin size="small" />
          ) : (
            isUser ? (
              <div style={{
                fontSize: '14px',
              }}>
                {msg.content}
              </div>
            ) : (
              <div className="markdown-content" style={{ color: token.colorText }}>
                <DocumentContent
                  document={{
                    content: msg.content
                  }}
                  owner=''
                  name=''
                  token={token}
                ></DocumentContent>
              </div>
            )
          )}
        </div>
      </div>
    );
  };

  // 发送消息的处理函数
  const handleSendMessage = async (content: string = message, init: boolean = false) => {
    if (!content.trim() && init == false) return;
    if (!init) {
      // 使用函数式更新确保获取最新状态
      setMessages(prevMessages => [
        ...prevMessages,
        {
          content,
          sender: 'user'
        }
      ]);
      // 清空输入框
      setMessage('');
    }

    if (loading) {
      return;
    }

    // 创建AI消息对象
    const aiMessage = {
      content: '',
      sender: 'ai' as const,
      loading: true
    };

    // 添加AI消息到消息列表
    setMessages(prevMessages => [...prevMessages, aiMessage]);
    setLoading(true);
    setFileListLoading(true); // 开始文件列表加载

    let aiResponseContent = '';

    try {
      const stream = fetchSSE(API_URL + '/api/Chat/Completions', {
        chatShareMessageId,
        question: content,
      });

      for await (const chunk of stream) {
        if (chunk.type === 'message') {
          aiResponseContent += chunk?.content ?? '';
          // 使用函数式更新获取最新状态，创建新的消息数组
          setMessages(prevMessages => {
            // 创建新的消息数组
            const newMessages = [...prevMessages];
            // 更新最后一条AI消息
            const lastMessage = newMessages[newMessages.length - 1];
            if (lastMessage && lastMessage.sender === 'ai') {
              newMessages[newMessages.length - 1] = {
                ...lastMessage,
                content: aiResponseContent,
                loading: false
              };
            }
            return newMessages;
          });
        } else if (chunk.type === 'tool') {
          const files = chunk.content.map((x: string) => {
            const value = x.split('/');
            const title = value[value.length - 1];
            return {
              path: x,
              title,
            }
          });
          setReferenceFiles([...files]);
        }
      }
    } catch (error) {
      console.error('流式响应出错:', error);
      messageApi.error('获取回复时发生错误');
    } finally {
      setLoading(false);
      setFileListLoading(false); // 结束文件列表加载
    }
  };

  // 添加自动滚动到底部的函数
  const scrollToBottom = () => {
    if (messagesContainerRef.current) {
      const container = messagesContainerRef.current;
      container.scrollTop = container.scrollHeight;
    }
  };

  // 监听消息变化，自动滚动到底部
  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  // 根据文件扩展名获取对应的图标放入组件内
  const getFileIcon = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase();

    switch (extension) {
      case 'md':
      case 'markdown':
        return <FileMarkdownOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
      case 'svg':
        return <FileImageOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      case 'xlsx':
      case 'xls':
      case 'csv':
        return <FileExcelOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      case 'doc':
      case 'docx':
        return <FileWordOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      case 'pdf':
        return <FilePdfOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      case 'json':
        return <CodeOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      case 'js':
      case 'ts':
      case 'tsx':
      case 'jsx':
      case 'py':
      case 'java':
      case 'c':
      case 'cpp':
      case 'cs':
      case 'go':
      case 'rb':
      case 'php':
      case 'html':
      case 'css':
      case 'scss':
      case 'less':
        return <FileOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
      default:
        return <FileUnknownOutlined style={{ color: token.colorPrimary, fontSize: token.fontSizeLG }} />;
    }
  };

  // 根据文件扩展名获取对应的语言名称
  const getLanguageFromExtension = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase();

    switch (extension) {
      case 'js': return 'javascript';
      case 'ts': return 'typescript';
      case 'tsx': return 'tsx';
      case 'jsx': return 'jsx';
      case 'py': return 'python';
      case 'java': return 'java';
      case 'c': return 'c';
      case 'cpp': return 'cpp';
      case 'cs': return 'csharp';
      case 'go': return 'go';
      case 'rb': return 'ruby';
      case 'php': return 'php';
      case 'html': return 'html';
      case 'css': return 'css';
      case 'scss': return 'scss';
      case 'less': return 'less';
      case 'json': return 'json';
      case 'md': return 'markdown';
      default: return 'text';
    }
  };

  return (
    <Layout style={{ minHeight: '100vh', backgroundColor: token.colorBgLayout }}>
      <Row >
        <Col xs={24} sm={24} md={14} lg={14} xl={14}>
          <div style={{
            height: '100vh',
            display: 'flex',
            flexDirection: 'column',
          }}>
            <div style={{
              padding: `${token.paddingSM}px ${token.paddingMD}px`,
              borderBottom: `1px solid ${token.colorBorderSecondary}`,
              backgroundColor: token.colorBgContainer,
              display: 'flex',
              alignItems: 'center',
              borderTopLeftRadius: token.borderRadiusLG,
              borderTopRightRadius: token.borderRadiusLG,
            }}>
              <a href="/" style={{
                display: 'flex',
                alignItems: 'center',
                fontSize: '18px',
                fontWeight: 'bold',
                color: token.colorPrimary,
                textDecoration: 'none',
                cursor: 'pointer'
              }}>
                <span>OpenDeepWiki</span>
              </a>
              <Button type='text'
                onClick={() => {
                  window.open(homepage)
                }}
              >
                <GithubFilled />
              </Button>
            </div>

            <div
              ref={messagesContainerRef}
              style={{
                flex: 1,
                overflowY: 'auto',
                padding: token.paddingMD,
                backgroundColor: token.colorBgContainer,
              }}
            >
              {messages.length === 0 ? (
                <Empty
                  description="开始一个新的对话"
                  image={Empty.PRESENTED_IMAGE_SIMPLE}
                />
              ) : (
                messages.map((msg, index) => renderMessage(msg, index))
              )}
            </div>

            {/* 底部输入区域 */}
            {/* <div
              style={{
                padding: token.paddingSM,
                backgroundColor: token.colorBgElevated,
                borderTop: `1px solid ${token.colorBorderSecondary}`,
                borderBottomLeftRadius: token.borderRadiusLG,
                borderBottomRightRadius: token.borderRadiusLG,
              }}
            >
              <div style={{ display: 'flex', gap: token.marginXS }}>
                <TextArea
                  value={message}
                  onChange={(e) => setMessage(e.target.value)}
                  placeholder="输入您的问题..."
                  autoSize={{ minRows: 1, maxRows: 3 }}
                  onPressEnter={(e) => {
                    if (!e.shiftKey) {
                      e.preventDefault();
                      handleSendMessage();
                    }
                  }}
                  style={{ flex: 1 }}
                  disabled={loading}
                />
                <Button
                  type="primary"
                  icon={<SendOutlined />}
                  onClick={() => handleSendMessage()}
                  loading={loading}
                  disabled={!message.trim()}
                  style={{ height: 'auto' }}
                >
                  发送
                </Button>
              </div>
            </div> */}
          </div>
        </Col>

        <Col xs={24} sm={24} md={10} lg={10} xl={10}>
          <div style={{
            height: '100vh',
            overflow: 'auto',
            backgroundColor: token.colorBgContainer,
            borderLeft: `1px solid ${token.colorBorderSecondary}`,
            display: 'flex',
            flexDirection: 'column',
          }}>
            <div style={{
              padding: `${token.paddingSM}px ${token.paddingMD}px`,
              borderBottom: `1px solid ${token.colorBorderSecondary}`,
              display: 'flex',
              alignItems: 'center',
              backgroundColor: token.colorBgElevated,
            }}>
              <FileTextOutlined style={{ marginRight: token.marginXS, color: token.colorPrimary }} />
              <Text strong>引用文件</Text>
              {fileListLoading && <Spin size="small" style={{ marginLeft: 'auto' }} />}
            </div>

            {fileListLoading ? (
              <div style={{ padding: token.paddingLG, display: 'flex', justifyContent: 'center' }}>
                <Spin tip="加载引用文件..." />
              </div>
            ) : referenceFiles.length > 0 ? (
              <>
                <List
                  itemLayout="horizontal"
                  dataSource={referenceFiles}
                  renderItem={(item) => (
                    <List.Item
                      style={{
                        padding: `${token.paddingSM}px ${token.paddingMD}px`,
                        transition: 'all 0.3s',
                        cursor: 'pointer',
                        borderRadius: token.borderRadiusSM,
                        margin: `${token.marginXS}px ${token.marginSM}px`,
                        boxShadow: `0 1px 2px 0 ${token.colorBorderSecondary}`,
                        backgroundColor: selectedFile === item.path ? token.colorBgTextHover : token.colorBgContainer,
                      }}
                      className="reference-file-item"
                      onClick={async () => {
                        setFileContentLoading(true)
                        try {
                          const { data } = await getFileContent(warehouseId, item.path);
                          setSelectedFile(item.path)
                          setFileContent(data.data);
                        } finally {
                          setFileContentLoading(false)
                        }
                      }}
                      onMouseEnter={(e) => {
                        if (selectedFile !== item.path) {
                          e.currentTarget.style.backgroundColor = token.colorBgTextHover;
                        }
                        e.currentTarget.style.boxShadow = `0 3px 6px -4px ${token.colorBgMask}`;
                      }}
                      onMouseLeave={(e) => {
                        if (selectedFile !== item.path) {
                          e.currentTarget.style.backgroundColor = token.colorBgContainer;
                        }
                        e.currentTarget.style.boxShadow = `0 1px 2px 0 ${token.colorBorderSecondary}`;
                      }}
                    >
                      <List.Item.Meta
                        avatar={
                          <div style={{
                            backgroundColor: token.colorPrimaryBg,
                            borderRadius: token.borderRadiusSM,
                            width: 40,
                            height: 40,
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            boxShadow: `0 2px 4px 0 ${token.colorBorderSecondary}`,
                          }}>
                            {getFileIcon(item.title)}
                          </div>
                        }
                        title={
                          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                            <Text strong style={{ fontSize: token.fontSizeSM }}>{item.title}</Text>
                            <Tooltip title="查看文件">
                              <Button
                                type="text"
                                size="small"
                                icon={<FileOutlined />}
                                style={{
                                  opacity: 0.7,
                                  color: token.colorTextSecondary,
                                }}
                                onClick={async (e) => {
                                  e.stopPropagation();
                                  setFileContentLoading(true)
                                  try {
                                    const { data } = await getFileContent(warehouseId, item.path);
                                    setSelectedFile(item.path)
                                    setFileContent(data.data);
                                  } finally {
                                    setFileContentLoading(false)
                                  }
                                }}
                              />
                            </Tooltip>
                          </div>
                        }
                        description={
                          <Text
                            type="secondary"
                            ellipsis={{ tooltip: item.path }}
                            style={{ fontSize: token.fontSize * 0.85 }}
                          >
                            {item.path}
                          </Text>
                        }
                      />
                    </List.Item>
                  )}
                  style={{
                    maxHeight: '30%',
                    overflowY: 'auto',
                    padding: `${token.paddingXS}px 0`,
                    flex: '0 0 auto',
                  }}
                />

                {/* 文件内容区域 */}
                {fileContent && (
                  <div style={{
                    flex: '1 1 auto',
                    padding: token.paddingSM,
                    borderTop: `1px solid ${token.colorBorderSecondary}`,
                    display: 'flex',
                    flexDirection: 'column'
                  }}>
                    <div style={{
                      padding: `${token.paddingXS}px ${token.paddingSM}px`,
                      marginBottom: token.marginSM,
                      backgroundColor: token.colorBgElevated,
                      borderRadius: token.borderRadiusSM,
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'space-between'
                    }}>
                      <Text strong style={{ fontSize: token.fontSizeSM }}>
                        {selectedFile ? selectedFile.split('/').pop() : '文件内容'}
                      </Text>
                      {selectedFile && (
                        <Tooltip title="复制路径">
                          <Button
                            type="text"
                            size="small"
                            icon={<CopyOutlined />}
                            onClick={() => {
                              navigator.clipboard.writeText(selectedFile);
                              messageApi.success('路径已复制');
                            }}
                          />
                        </Tooltip>
                      )}
                    </div>

                    {fileContentLoading ? (
                      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', flex: 1 }}>
                        <Spin tip="加载文件内容..." />
                      </div>
                    ) : selectedFile ? (
                      <div
                        style={{
                          flex: 1,
                          overflowY: 'auto',
                          borderRadius: token.borderRadiusSM,
                          border: `1px solid ${token.colorBorderSecondary}`,
                          backgroundColor: token.colorBgElevated
                        }}
                      >
                        <SyntaxHighlighter
                          language={getLanguageFromExtension(selectedFile.split('/').pop() || '')}
                          PreTag="div"
                          customStyle={{
                            margin: 0,
                            padding: token.paddingSM,
                            fontSize: token.fontSize * 0.9,
                            backgroundColor: 'transparent',
                            height: '100%'
                          }}
                        >
                          {fileContent}
                        </SyntaxHighlighter>
                      </div>
                    ) : (
                      <Empty
                        image={Empty.PRESENTED_IMAGE_SIMPLE}
                        description="请选择一个文件查看内容"
                        style={{ margin: token.marginMD }}
                      />
                    )}
                  </div>)}
              </>
            ) : (
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description={
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                    <Text type="secondary">暂无引用文件</Text>
                    <Text type="secondary" style={{ fontSize: token.fontSize * 0.85 }}>
                      系统会在回答过程中自动识别相关文件
                    </Text>
                  </div>
                }
                style={{
                  margin: token.marginLG,
                  padding: token.paddingLG,
                  height: 'calc(100% - 150px)',
                  display: 'flex',
                  flexDirection: 'column',
                  justifyContent: 'center',
                  alignItems: 'center',
                }}
              />
            )}
          </div>
        </Col>
      </Row>
    </Layout>
  );
} 