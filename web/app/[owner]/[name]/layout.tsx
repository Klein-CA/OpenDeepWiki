'use client'
import {
  Layout,
  Typography,
  Spin,
  Space,
  theme,
  ConfigProvider,
  Flex,
  Avatar,
  Breadcrumb,
  Divider,
  Button,
  FloatButton,
  Dropdown
} from 'antd';
import {
  FolderOutlined,
  FileTextOutlined,
  GithubOutlined,
  HomeOutlined,
  BookOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  MoreOutlined,
  ShareAltOutlined,
  EditOutlined,
  SettingOutlined
} from '@ant-design/icons';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useEffect, useState } from 'react';
import { documentCatalog } from '../../services/warehouseService';
import AIInputBar from '../../components/AIInputBar';

const { Header, Content, Sider } = Layout;
const { Title, Text } = Typography;

interface DocumentCatalogResponse {
  key: string;
  label: string;
  url: string;
  order: number;
  children?: DocumentCatalogResponse[];
}

const generateMenuItems = (
  data: DocumentCatalogResponse,
  parentKey: string = '',
  owner: string,
  name: string
) => {
  const key = parentKey ? `${parentKey}-${data.key}` : data.key;
  const item = {
    key,
    icon: data.children?.length ? <FolderOutlined /> : <FileTextOutlined />,
    label: data.children?.length
      ? data.label
      : <Link href={`/${owner}/${name}/${data.url}`}>{data.label}</Link>,
  };

  if (data.children?.length) {
    return {
      ...item,
      children: data.children
        .sort((a, b) => a.order - b.order)
        .map(child => generateMenuItems(child, key, owner, name))
    };
  }

  return item;
};

export default function RepositoryLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const { token } = theme.useToken();

  const pathParts = pathname.split('/').filter(Boolean);
  const owner = pathParts[0] || '';
  const name = pathParts[1] || '';
  const currentPath = pathParts.slice(2).join('/');

  const [catalogData, setCatalogData] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [lastUpdated, setLastUpdated] = useState('');
  const [collapsed, setCollapsed] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  const selectedKey = pathname.includes('/') ? 'docs' : 'overview';

  useEffect(() => {
    if (!owner || !name) return;

    const fetchDocumentCatalog = async () => {
      setLoading(true);
      try {
        const response = await documentCatalog(owner, name);
        setCatalogData(response.data);
        setLastUpdated(response.data.lastUpdate);
      } catch (error) {
        console.error('Failed to fetch document catalog:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchDocumentCatalog();
  }, [owner, name, selectedKey]);

  // Check if the screen size is mobile
  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth <= 768);
    };

    checkMobile();
    window.addEventListener('resize', checkMobile);

    return () => {
      window.removeEventListener('resize', checkMobile);
    };
  }, []);

  // Automatically collapse sidebar on mobile
  useEffect(() => {
    if (isMobile) {
      setCollapsed(true);
    }
  }, [isMobile]);

  const renderSidebarItem = (item: DocumentCatalogResponse, level = 0) => {
    const isActive = pathname.includes(`/${item.url}`);

    const style = {
      padding: `${token.paddingXS}px ${token.paddingLG}px`,
      paddingLeft: `${token.paddingLG + level * token.paddingMD}px`,
      color: isActive ? token.colorPrimary : token.colorText,
      cursor: 'pointer',
      backgroundColor: isActive ? token.colorBgTextActive : 'transparent',
      transition: `all ${token.motionDurationMid}`,
      display: 'flex',
      alignItems: 'center',
      textDecoration: 'none',
      fontWeight: isActive ? 500 : 400,
      margin: `5px 0`,
      borderRadius: token.borderRadiusLG,
      fontSize: token.fontSizeSM,
      lineHeight: token.lineHeight,
    };

    const iconStyle = {
      marginRight: token.marginXS,
      fontSize: token.fontSizeSM,
    };

    return (
      <div key={item.key}>
        {item.children?.length ? (
          <>
            <Link
              href={`/${owner}/${name}/${item.url}`}
              style={style}>
              <FolderOutlined style={iconStyle} />
              <span>{item.label}</span>
            </Link>
            {item.children.sort((a, b) => a.order - b.order).map(child =>
              renderSidebarItem(child, level + 1)
            )}
          </>
        ) : (
          <Link
            href={`/${owner}/${name}/${item.url}`}
            style={style}
          >
            <FileTextOutlined style={iconStyle} />
            <span>{item.label}</span>
          </Link>
        )}
      </div>
    );
  };

  const generateBreadcrumb = () => {
    const items = [
      {
        title: <Link href="/"><HomeOutlined /></Link>,
      },
      {
        title: <Link href={`/${owner}`}>{owner}</Link>,
      },
      {
        title: <Link href={`/${owner}/${name}`}>{name}</Link>,
      }
    ];

    if (currentPath) {
      items.push({
        title: <span>{currentPath}</span>,
      });
    }

    return items;
  };

  // Define floating menu items
  const floatingMenuItems = [
    {
      key: 'edit',
      label: '编辑文档',
      icon: <EditOutlined />,
      onClick: () => {
        // Implement edit action
        console.log('Edit document');
      }
    },
    {
      key: 'share',
      label: '分享文档',
      icon: <ShareAltOutlined />,
      onClick: () => {
        // Implement share action
        console.log('Share document');
      }
    },
    {
      key: 'settings',
      label: '文档设置',
      icon: <SettingOutlined />,
      onClick: () => {
        // Implement settings action
        console.log('Document settings');
      }
    }
  ];

  return (
    <ConfigProvider
      theme={{
        components: {
          Layout: {
            headerBg: token.colorBgElevated,
            siderBg: token.colorBgContainer,
            bodyBg: token.colorBgLayout,
          },
          FloatButton: {
            colorPrimary: token.colorPrimary,
          }
        },
      }}
    >
      <Layout style={{ minHeight: '100vh' }}>
        <Header style={{
          padding: 0,
          background: token.colorBgContainer,
          position: 'fixed',
          width: '100%',
          zIndex: 1000,
          borderBottom: `1px solid ${token.colorBorderSecondary}`,
          boxShadow: token.boxShadowSecondary
        }}>
          <Flex align="center" justify="space-between" style={{ height: '100%' }}>
            <Flex align="center" >
              <Button
                type="text"
                icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                onClick={() => setCollapsed(!collapsed)}
                style={{
                  fontSize: token.fontSizeLG,
                  marginRight: token.marginXS,
                }}
              />
              <BookOutlined style={{ fontSize: token.fontSizeHeading3, color: token.colorPrimary }} />
              <Title
                level={4}
                onClick={() => {
                  catalogData?.git && window.open(catalogData.git, '_blank');
                }}
                style={{
                  margin: 0,
                  color: token.colorText,
                  fontSize: token.fontSizeHeading4,
                  lineHeight: 1.2,
                  cursor: catalogData?.git ? 'pointer' : 'default',
                  display: 'flex',
                  alignItems: 'center',
                }}
              >
                <span style={{ color: token.colorPrimary }}>
                  {owner}
                </span>
                <span style={{ margin: `0 ${token.marginXXS}px`, color: token.colorTextSecondary }}>/</span>
                <span>{name}</span>
                {catalogData?.git && (
                  <GithubOutlined style={{ marginLeft: token.marginSM, fontSize: token.fontSizeLG }} />
                )}
              </Title>
            </Flex>

            <Space>
              {lastUpdated && (
                <Text type="secondary" style={{ fontSize: token.fontSizeSM }}>
                  最近更新: {lastUpdated}
                </Text>
              )}
            </Space>
          </Flex>
        </Header>

        <Layout style={{ marginTop: 64 }}>
          <Sider
            width={260}
            collapsible
            collapsed={collapsed}
            onCollapse={setCollapsed}
            trigger={null}
            breakpoint="lg"
            collapsedWidth={0}
            style={{
              background: token.colorBgContainer,
              overflow: 'auto',
              height: 'calc(100vh - 64px)',
              position: 'fixed',
              left: 0,
              top: 64,
              bottom: 0,
              borderRight: `1px solid ${token.colorBorderSecondary}`,
              transition: `all ${token.motionDurationMid}`,
              zIndex: 999,
            }}
          >
            <div style={{ padding: `${token.paddingMD}px 0` }}>
              <Flex
                vertical
                gap={token.marginSM}
                style={{ padding: `0 ${token.paddingXS}px` }}
              >
                <Link
                  href={`/${owner}/${name}`}
                  style={{
                    padding: `${token.paddingXS}px ${token.paddingLG}px`,
                    color: pathname === `/${owner}/${name}` ? token.colorPrimary : token.colorText,
                    backgroundColor: pathname === `/${owner}/${name}` ? token.colorBgTextActive : 'transparent',
                    fontWeight: pathname === `/${owner}/${name}` ? 500 : 400,
                    display: 'flex',
                    alignItems: 'center',
                    textDecoration: 'none',
                    borderRadius: token.borderRadiusLG,
                    marginBottom: token.marginXS,
                  }}
                >
                  <HomeOutlined style={{ marginRight: token.marginXS }} />
                  <span>概览</span>
                </Link>

                <Divider style={{ margin: `0`, padding: '0' }} />

                {loading ? (
                  <Flex justify="center" align="center" style={{ padding: token.paddingMD }}>
                    <Spin size="small" />
                  </Flex>
                ) : (
                  catalogData?.items?.map(item => renderSidebarItem(item))
                )}

                <Link
                  href={`/${owner}/${name}/changelog`}
                  style={{
                    padding: `${token.paddingXS}px ${token.paddingLG}px`,
                    color: pathname === `/${owner}/${name}/changelog` ? token.colorPrimary : token.colorText,
                    backgroundColor: pathname === `/${owner}/${name}/changelog` ? token.colorBgTextActive : 'transparent',
                    fontWeight: pathname === `/${owner}/${name}/changelog` ? 500 : 400,
                    display: 'flex',
                    alignItems: 'center',
                    textDecoration: 'none',
                    borderRadius: token.borderRadiusLG,
                    marginBottom: token.marginXS,
                  }}
                >
                  <span>更新日志</span>
                </Link>
              </Flex>
            </div>
          </Sider>

          <Content style={{
            marginLeft: collapsed ? 0 : 260,
            padding: token.paddingLG,
            background: token.colorBgContainer,
            minHeight: 'calc(100vh - 64px)',
            transition: `all ${token.motionDurationMid}`,
            position: 'relative',
          }}>
            <Breadcrumb
              items={generateBreadcrumb()}
              style={{
                marginBottom: token.marginLG,
                fontSize: token.fontSizeSM
              }}
            />

            <div style={{
              background: token.colorBgContainer,
              padding: token.paddingLG,
              borderRadius: token.borderRadiusLG,
              boxShadow: token.boxShadowTertiary
            }}>
              {children}
            </div>
          </Content>
        </Layout>
      </Layout>
      <AIInputBar 
        owner={owner}
        name={name}
      />
    </ConfigProvider>
  );
} 