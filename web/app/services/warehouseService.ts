import { fetchApi, ApiResponse, API_URL } from './api';
import { Repository, RepositoryFormValues } from '../types';

/**
 * Repository submission interface
 */
export interface WarehouseSubmitRequest {
  address: string;
  type: string;
  branch: string;
  prompt: string;
  model: string;
  openAIKey: string;
  openAIEndpoint: string;
}

/**
 * Response structure for warehouse list
 */
export interface WarehouseListResponse {
  total: number;
  items: Repository[];
}


/**
 * Submit a new repository to the warehouse
 * 这个函数仍然需要在客户端使用
 */
export async function submitWarehouse(
  data: RepositoryFormValues
): Promise<ApiResponse<Repository>> {
  // @ts-ignore
  return fetchApi<Repository>(API_URL + '/api/Warehouse/SubmitWarehouse', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

/**
 * Get warehouse list
 * 此函数可在服务器组件中使用
 */
export async function getWarehouse(page: number, pageSize: number): Promise<ApiResponse<WarehouseListResponse>> {
  // @ts-ignore
  return fetchApi<WarehouseListResponse>(API_URL + '/api/Warehouse/WarehouseList?page=' + page + '&pageSize=' + pageSize, {
    method: 'GET',
    // 添加缓存控制使其适用于服务器组件
    cache: 'no-store'
  });
}

/**
 * 获取文档目录
 * 此函数可在服务器组件中使用
 */
export async function documentCatalog(organizationName: string, name: string): Promise<any> {
  // @ts-ignore
  return fetchApi<any>(API_URL + '/api/DocumentCatalog/DocumentCatalogs?organizationName=' + organizationName + '&name=' + name, {
    method: 'GET',
    // 添加缓存控制使其适用于服务器组件
    cache: 'no-store'
  });
}

/**
 * 根据ID获取文档
 * 此函数可在服务器组件中使用
 */
export async function documentById(owner: string, name: string, path: string): Promise<any> {
  // @ts-ignore
  return fetchApi<any>(API_URL + '/api/DocumentCatalog/DocumentById?owner=' + owner + '&name=' + name + '&path=' + path, {
    method: 'GET',
    // 添加缓存控制使其适用于服务器组件
    cache: 'no-store'
  });
}

/**
 * 获取仓库概览信息
 * 此函数可在服务器组件中使用
 */
export async function getWarehouseOverview(owner: string, name: string) {
  // @ts-ignore
  return fetchApi<any>(API_URL + '/api/Warehouse/WarehouseOverview?owner=' + owner + '&name=' + name, {
    method: 'GET',
    // 添加缓存控制使其适用于服务器组件
    cache: 'no-store'
  });
}

/**
 * 获取最近的仓库信息
 * 此函数可在服务器组件中使用
 */
export async function getLastWarehouse(address: string) {
  // @ts-ignore
  return fetchApi<any>(API_URL + '/api/Warehouse/LastWarehouse?address=' + address, {
    method: 'GET',
    // 添加缓存控制使其适用于服务器组件
    cache: 'no-store'
  });
}

/**
 * 获取更新日志
 * 此函数可在服务器组件中使用
 */
export async function getChangeLog(owner: string, name: string) {
  // @ts-ignore
  return fetchApi<any>(API_URL + '/api/Warehouse/ChangeLog?owner=' + owner + '&name=' + name, {
    method: 'GET',
    // 添加缓存控制使其适用于服务器组件
    cache: 'no-store'
  });
}


/**
 * 
 * @param warehouseId 
 * @param path 
 * @returns 
 */
export async function getFileContent(warehouseId: string, path: string) {
  return fetchApi<any>(API_URL + `/api/Warehouse/FileContent?warehouseId=${warehouseId}&path=${path}`, {
    method: 'GET',
    cache: 'no-cache'
  })
}