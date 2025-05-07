import { message } from 'antd';

interface ApiResponse<T> {
  data?: T;
  error?: string;
  success: boolean;
  isSuccess?: boolean;
  message?: string;
}

/**
 * 服务器端API请求处理程序 - 无UI交互
 */
async function serverFetchApi<T>(
  url: string,
  options?: RequestInit
): Promise<ApiResponse<T>> {
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      const errorMessage = errorData.message || `请求失败: ${response.status}`;
      return { success: false, error: errorMessage, message: errorMessage };
    }

    // 根据响应类型返回
    if (response.headers.get("content-type") === "text/markdown") {
      const data = await response.text();
      // @ts-ignore
      return { data, success: true, isSuccess: true };
    } else {
      const data = await response.json();
      return { data, success: true, isSuccess: true };
    }
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : '请求异常';
    return { success: false, error: errorMessage, message: errorMessage };
  }
}

/**
 * 客户端API请求处理程序 - 包含UI交互
 */
async function clientFetchApi<T>(
  url: string,
  options?: RequestInit
): Promise<ApiResponse<T>> {
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      const errorMessage = errorData.message || `请求失败: ${response.status}`;
      message.error(errorMessage);
      return { success: false, error: errorMessage, message: errorMessage };
    }

    // 根据响应类型返回
    if (response.headers.get("content-type") === "text/markdown") {
      const data = await response.text();
      // @ts-ignore
      return { data, success: true, isSuccess: true };
    } else {
      const data = await response.json();
      return { data, success: true, isSuccess: true };
    }
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : '请求异常';
    message.error(errorMessage);
    return { success: false, error: errorMessage, message: errorMessage };
  }
}

/**
 * 通用API请求处理程序
 * 会根据环境自动选择使用服务器端或客户端请求
 */
async function fetchApi<T>(
  url: string,
  options?: RequestInit
): Promise<ApiResponse<T>> {
  // 判断是否在客户端环境
  if (typeof window !== 'undefined') {
    // 客户端环境
    return clientFetchApi<T>(url, options);
  } else {
    // 服务器端环境
    return serverFetchApi<T>(url, options);
  }
}

// SSE 辅助函数
async function* fetchSSE(url: string, data: any): AsyncIterableIterator<any> {
  const token = localStorage.getItem("token");
  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const response = await window.fetch(url, {
    headers,
    method: "POST",
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errorText = await response.text();
    try {
      const json = JSON.parse(errorText);
      if (!json.success) {
        console.log(json);
        
      }
      throw new Error(json.message || 'API请求失败');
    } catch {
      throw new Error(errorText || '网络请求失败');
    }
  }

  const reader = response.body!.getReader();
  const decoder = new TextDecoder("utf-8");

  try {
    while (true) {
      const { done, value } = await reader.read();
      if (done) break;

      const chunk = decoder.decode(value, { stream: true });
      const events = chunk.split("\n\n");

      for (const event of events) {
        if (event.trim()) {
          const dataLine = event.split("\n").find(line => line.startsWith("data:"));
          if (dataLine) {
            const jsonData = dataLine.replace("data:", "").trim();
            if (jsonData === "[done]") {
              break;
            }
            yield JSON.parse(jsonData);
          }
        }
      }
    }
  } finally {
    reader.cancel();
  }
}
const API_URL = process.env.NEXT_PUBLIC_API_URL || process.env.API_URL || '';

export { fetchApi, serverFetchApi, clientFetchApi, API_URL ,fetchSSE};
export type { ApiResponse }; 