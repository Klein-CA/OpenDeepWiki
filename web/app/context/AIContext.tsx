'use client';

import React, { createContext, useContext, useState, ReactNode } from 'react';
import { createChatShareMessage } from '../services/chatShareMessageServce';
import { useRouter } from 'next/navigation';

interface AIContextType {
  // AI会话状态
  isLoading: boolean;
  sendMessage: (message: string, deepResearch: boolean, owner: string, name: string) => Promise<void>;
}

const defaultContext: AIContextType = {
  isLoading: false,
  sendMessage: async () => {},
};

const AIContext = createContext<AIContextType>(defaultContext);

export const useAI = () => useContext(AIContext);

interface AIProviderProps {
  children: ReactNode;
}

export const AIProvider: React.FC<AIProviderProps> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(false);
  const router = useRouter();

  // 发送消息给AI服务
  const sendMessage = async (message: string, deepResearch: boolean, owner: string, name: string) => {
    try {
      setIsLoading(true);
      console.log(owner, name);

      const warehouseId = await createChatShareMessage({
        isDeep: deepResearch,
        owner: owner,
        name: name,
        message: message,
      })
      
      window.open(`/search/${warehouseId.data.data}`)

    } catch (error) {
      console.error('发送消息错误:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const value = {
    isLoading,
    sendMessage,
  };

  return <AIContext.Provider value={value}>{children}</AIContext.Provider>;
};

export default AIProvider; 