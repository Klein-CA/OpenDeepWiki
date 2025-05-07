import { useState, useEffect } from 'react';
import { Modal, Form, Input, Button, message, Spin, Typography, Descriptions, Tag, Space, Result, Row, Col, theme } from 'antd';
import { 
  SearchOutlined, 
  GithubOutlined, 
  BranchesOutlined, 
  ClockCircleOutlined, 
  InfoCircleOutlined,
  ExclamationCircleOutlined,
  SyncOutlined,
  CheckCircleOutlined,
  StopOutlined,
  LockOutlined,
  QuestionCircleOutlined
} from '@ant-design/icons';
import { getLastWarehouse } from '../services/warehouseService';
import { Repository } from '../types';

const { Text, Title, Paragraph } = Typography;
const { useToken } = theme;

interface LastRepoModalProps {
  open: boolean;
  onCancel: () => void;
}

const LastRepoModal: React.FC<LastRepoModalProps> = ({ open, onCancel }) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [repository, setRepository] = useState<Repository | null>(null);
  const [searched, setSearched] = useState(false);
  const { token } = useToken();

  useEffect(() => {
    if (!open) {
      form.resetFields();
      setRepository(null);
      setSearched(false);
    }
  }, [open, form]);

  const handleSearch = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);
      setSearched(false);
      
      try {
        const response = await getLastWarehouse(values.address);
        if (response.success && response.data) {
          setRepository(response.data);
          setSearched(true);
        } else {
          message.error('查询失败: ' + (response.error || '未找到相关仓库'));
          setRepository(null);
        }
      } catch (error) {
        console.error('查询仓库出错:', error);
        message.error('查询仓库出错，请稍后重试');
        setRepository(null);
      } finally {
        setLoading(false);
      }
    } catch (error) {
      // 表单验证失败
    }
  };

  const handleCancel = () => {
    form.resetFields();
    setRepository(null);
    setSearched(false);
    onCancel();
  };

  // 获取仓库状态文本
  const getStatusText = (status: number) => {
    const statusMap: Record<number, { text: string; color: string; icon: React.ReactNode }> = {
      0: { text: '待处理', color: 'warning', icon: <ClockCircleOutlined /> },
      1: { text: '处理中', color: 'processing', icon: <SyncOutlined spin /> },
      2: { text: '已完成', color: 'success', icon: <CheckCircleOutlined /> },
      3: { text: '已取消', color: 'default', icon: <StopOutlined /> },
      4: { text: '未授权', color: 'purple', icon: <LockOutlined /> },
      99: { text: '已失败', color: 'error', icon: <ExclamationCircleOutlined /> },
    };
    return statusMap[status] || { text: '未知状态', color: 'default', icon: <QuestionCircleOutlined /> };
  };

  const contentStyle = {
    backgroundColor: token.colorBgContainer,
    borderRadius: token.borderRadiusLG,
    padding: token.paddingMD,
    marginTop: token.marginMD,
    boxShadow: token.boxShadowTertiary
  };

  return (
    <Modal
      title={<Typography.Title level={4} style={{ margin: 0 }}>查询上次提交的仓库</Typography.Title>}
      open={open}
      onCancel={handleCancel}
      footer={null}
      width={{ xs: '90%', sm: 520, md: 600 }}
      centered
    >
      <Form form={form} layout="vertical">
        <Form.Item
          name="address"
          label={<Text strong>仓库地址</Text>}
          rules={[{ required: true, message: '请输入仓库地址' }]}
          tooltip={{ title: '输入您的Git仓库完整URL', icon: <InfoCircleOutlined /> }}
        >
          <Input 
            placeholder="请输入Git仓库地址，例如: https://github.com/username/repo" 
            prefix={<GithubOutlined style={{ color: token.colorTextSecondary }} />}
            size="large"
            autoFocus
            allowClear
            onPressEnter={handleSearch}
          />
        </Form.Item>
      </Form>
      
      <Row justify="center" style={{ marginTop: token.marginSM }}>
        <Col>
          <Button 
            type="primary" 
            icon={<SearchOutlined />} 
            onClick={handleSearch}
            loading={loading}
            size="large"
          >
            查询仓库
          </Button>
        </Col>
      </Row>

      {loading && (
        <div style={{ textAlign: 'center', padding: token.paddingLG }}>
          <Spin size="large" tip={<Text type="secondary">正在查询仓库信息...</Text>} />
        </div>
      )}

      {!loading && searched && repository && (
        <div style={contentStyle}>
          <Row justify="space-between" align="middle" style={{ marginBottom: token.marginSM }}>
            <Col>
              <Title level={5} style={{ margin: 0, color: token.colorTextHeading }}>查询结果</Title>
            </Col>
            <Col>
              <Tag color={getStatusText(repository.status).color} icon={getStatusText(repository.status).icon}>
                {getStatusText(repository.status).text}
              </Tag>
            </Col>
          </Row>
          
          <Descriptions 
            bordered 
            column={{ xs: 1, sm: 1 }} 
            size="small"
            labelStyle={{ backgroundColor: token.colorBgLayout, width: '25%' }}
            contentStyle={{ backgroundColor: token.colorBgContainer }}
          >
            <Descriptions.Item 
              label={<Text strong>仓库名称</Text>}
            >
              <Text>{repository.name}</Text>
            </Descriptions.Item>
            
            <Descriptions.Item 
              label={<Text strong>仓库地址</Text>}
            >
              <Paragraph 
                ellipsis={{ rows: 2, expandable: true, symbol: '展开' }}
                style={{ marginBottom: 0 }}
              >
                {repository.address}
              </Paragraph>
            </Descriptions.Item>
            
            <Descriptions.Item label={<Text strong>仓库类型</Text>}>
              <Tag icon={<GithubOutlined />} color="blue">{repository.type}</Tag>
            </Descriptions.Item>
            
            <Descriptions.Item label={<Text strong>分支</Text>}>
              <Tag icon={<BranchesOutlined />} color="cyan">{repository.branch}</Tag>
            </Descriptions.Item>
            
            {repository.error && (
              <Descriptions.Item 
                label={<Text strong type="danger">错误内容</Text>}
                contentStyle={{ backgroundColor: token.colorErrorBg }}
              >
                <Text type="danger">{repository.error}</Text>
              </Descriptions.Item>
            )}
          </Descriptions>
        </div>
      )}

      {!loading && searched && !repository && (
        <Result
          status="warning"
          title="未找到仓库信息"
          subTitle="请检查输入的仓库地址是否正确"
          icon={<ExclamationCircleOutlined style={{ color: token.colorWarning }} />}
          style={{ padding: token.paddingLG }}
        />
      )}
    </Modal>
  );
};

export default LastRepoModal;