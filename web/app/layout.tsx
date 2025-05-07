import { AntdRegistry } from '@ant-design/nextjs-registry';
import { ConfigProvider, theme } from 'antd';
import zhCN from 'antd/locale/zh_CN';
import './globals.css';
import OpenAIProvider from './context/OpenAIContext';
import AIProvider from './context/AIContext';
import '@ant-design/v5-patch-for-react-19';
import Script from 'next/script';


export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  // 从环境变量NEXT_PUBLIC_API_URL读取
  const apiUrl = process.env.NEXT_PUBLIC_API_URL || process.env.API_URL || '';

  return (
    <html lang="zh-CN">
      <head />
      <body >
        <Script id="api-url"
          type="text/javascript"
          strategy="afterInteractive"
          dangerouslySetInnerHTML={{
            __html: `
              console.log('API_URL', '${apiUrl}');
              window.API_URL = '${apiUrl}';
              `
          }}
        />
        <AntdRegistry>
          <ConfigProvider
            locale={zhCN}
            theme={{
              token: {
                colorPrimary: '#1677ff',
              },
              algorithm: theme.defaultAlgorithm,
            }}
          >
            <OpenAIProvider>
              <AIProvider>
                {children}
              </AIProvider>
            </OpenAIProvider>
          </ConfigProvider>
        </AntdRegistry>
      </body>
    </html>
  )
}