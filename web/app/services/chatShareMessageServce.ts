import { API_URL, fetchApi } from './api';

interface ChatShareMessageInput {
  isDeep: boolean;
  owner: string;
  name: string;
  message: string;
}
/**
 * Submit a new repository to the warehouse
 * 这个函数仍然需要在客户端使用
 */
export async function createChatShareMessage(
  data: ChatShareMessageInput
): Promise<any> {
  return await fetchApi<any>(API_URL + '/api/ChatShareMessage', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

/**
 * 
 * @param chatShareMessageId 
 * @param page 
 * @param pageSize 
 * @returns 
 */
export async function getChatShareMessageList(chatShareMessageId:string,page:number,pageSize:number){
  return await fetchApi<any>(API_URL +`/api/ChatShareMessage/List?chatShareMessageId=${chatShareMessageId}&page=${page}&pageSize=${pageSize}`)
}