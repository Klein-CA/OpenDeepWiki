'use client';

import React, { useState, useRef } from 'react';
import { Input, Button, Switch, Flex, Space, theme, Grid } from 'antd';
import type { InputRef } from 'antd';
import { SendOutlined, LoadingOutlined } from '@ant-design/icons';
import { useAI } from '../context/AIContext';

const { useBreakpoint } = Grid;

const AIInputBar: React.FC<{ owner: string, name: string }> = ({ owner, name }) => {
    const { token } = theme.useToken();
    const screens = useBreakpoint();
    const { isLoading, sendMessage } = useAI();
    const [inputValue, setInputValue] = useState('');
    const [deepResearch, setDeepResearch] = useState(false);
    const inputRef = useRef<InputRef>(null);

    const handleSend = async () => {
        if (inputValue.trim() && !isLoading) {
            await sendMessage(inputValue, deepResearch, owner, name);
            setInputValue('');
        }
    };

    const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    // 深入推理开关的标签
    const researchLabel = screens.sm ? '深入推理' : '';

    return (
        <div className="ai-input-container" style={{
            padding: token.paddingSM,
            backgroundColor: token.colorBgContainer,
            position: 'absolute',
            borderRadius: '10px',
            boxShadow: `rgb(189 188 188) 0px -2px 8px`,
            // 如果宽度小于700px，则宽度为100%
        }}>
            <Flex
                align="center"
                justify="center"

                style={{
                    maxWidth: '800px',
                    margin: '0 auto',
                    // 上下排
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                }}
            >
                <Input.TextArea
                    ref={inputRef}
                    value={inputValue}
                    onChange={(e) => setInputValue(e.target.value)}
                    onKeyDown={handleKeyDown}
                    placeholder="输入问题..."
                    autoSize={{ minRows: 1, maxRows: 4 }}
                    disabled={isLoading}
                    style={{
                        marginBottom: '10px',
                        resize: 'none',
                        outline: 'none',
                        border: 'none',
                        boxShadow: 'none',
                    }}
                />

                <Space
                    style={{
                        width: '100%',
                        display: 'flex',
                        flexDirection: 'row',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                    }}
                >
                    <Flex >
                        <Switch
                            size="small"
                            checked={deepResearch}
                            onChange={setDeepResearch}
                            disabled={isLoading}
                            style={{ marginRight: researchLabel ? token.marginXS : 0 }}
                        />
                        {researchLabel && (
                            <span style={{ fontSize: token.fontSizeSM, whiteSpace: 'nowrap' }}>
                                {researchLabel}
                            </span>
                        )}
                    </Flex>

                    <Button
                        type="primary"
                        icon={isLoading ? <LoadingOutlined /> : <SendOutlined />}
                        onClick={handleSend}
                        shape="circle"
                        size="large"
                        disabled={isLoading || !inputValue.trim()}
                        style={{
                            transition: `all ${token.motionDurationMid}`,
                        }}
                    />
                </Space>
            </Flex>
        </div>
    );
};

export default AIInputBar; 