'use client'

import { Suspense, useEffect, useState } from 'react';
import { getWarehouseOverview } from '../../services';
import { RepositoryView } from './RepositoryView';
import { ServerLoadingErrorState } from '../../components/document/ServerComponents';
import { usePathname } from 'next/navigation';

export default function RepositoryPage({ params }: any) {
  try {
    const pathname = usePathname();

    const pathParts = pathname.split('/').filter(Boolean);
    const owner = pathParts[0] || '';
    const name = pathParts[1] || '';

    if (!owner || !name) {
      throw new Error('Missing owner or repository name');
    }
    const [response, setResponse] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
      const fetchData = async () => {
        try {
          const data = await getWarehouseOverview(owner, name);
          setResponse(data);
        } catch (err) {
          setError(err);
        } finally {
          setLoading(false);
        }
      };

      fetchData();
    }, [owner, name]);

    if (loading) {
      return <ServerLoadingErrorState loading={true} owner={owner} name={name} />;
    }

    if (error) {
      return <ServerLoadingErrorState loading={false} error={'抱歉，获取仓库信息时发生错误'} owner={""} name={""} />;
    }

    // 如果获取数据失败，显示错误信息
    if (!response.success || !response.data) {
      return (
        <ServerLoadingErrorState
          loading={false}
          error={'抱歉，仓库不存在'}
          owner={owner}
          name={name}
        />
      );
    }

    // 将数据传递给客户端组件进行渲染
    return (
      <Suspense fallback={<ServerLoadingErrorState loading={true} owner={owner} name={name} />}>
        <RepositoryView
          owner={owner}
          name={name}
          document={response.data}
        />
      </Suspense>
    );
  } catch (error) {
    return (
      <ServerLoadingErrorState
        loading={false}
        error={'抱歉，获取仓库信息时发生错误'}
        owner={""}
        name={""}
      />
    );
  }
}
