using System.ComponentModel;
using LibGit2Sharp;

namespace KoalaWiki.Git;

/// <summary>
/// Git服务类，提供与Git仓库相关的操作。
/// </summary>
public class GitService
{
    /// <summary>
    /// 获取仓库的本地路径和组织名称。
    /// </summary>
    /// <param name="repositoryUrl">仓库的URL地址。</param>
    /// <returns>返回一个元组，包含本地路径和组织名称。</returns>
    private static (string localPath, string organization) GetRepositoryPath(string repositoryUrl)
    {
        // 解析仓库地址
        var uri = new Uri(repositoryUrl);
        // 得到组织名和仓库名称
        var segments = uri.Segments;
        var organization = segments[1].Trim('/');
        var repositoryName = segments[2].Trim('/').Replace(".git", "");

        // 拼接本地路径，默认使用"/repositories"
        var repositoryPath = Path.Combine(Constant.GitPath, organization, repositoryName);
        return (repositoryPath, organization);
    }

    /// <summary>
    /// 拉取指定仓库。
    /// </summary>
    /// <param name="repositoryUrl">仓库的URL地址。</param>
    /// <param name="userName">用户名，用于认证。</param>
    /// <param name="password">密码，用于认证。</param>
    /// <param name="branch">分支名称，默认为"master"。</param>
    /// <returns>返回包含仓库信息的<see cref="GitRepositoryInfo"/>对象。</returns>
    public static GitRepositoryInfo PullRepository(
        [Description("仓库地址")] string repositoryUrl,
        string userName = "",
        string password = "",
        [Description("分支")] string branch = "master")
    {
        var (localPath, organization) = GetRepositoryPath(repositoryUrl);

        var cloneOptions = new CloneOptions
        {
            FetchOptions =
            {
                CertificateCheck = (certificate, chain, errors) => true,
                Depth = 0,
            }
        };

        var names = repositoryUrl.Split('/');

        var repositoryName = names[^1].Replace(".git", "");

        // 判断仓库是否已经存在
        if (Directory.Exists(localPath))
        {
            // 获取当前仓库的git分支
            using var repo = new Repository(localPath);
            var branchName = repo.Head.FriendlyName;
            // 获取当前仓库的git版本
            var version = repo.Head.Tip.Sha;
            // 获取当前仓库的git提交时间
            var commitTime = repo.Head.Tip.Committer.When;
            // 获取当前仓库的git提交人
            var commitAuthor = repo.Head.Tip.Committer.Name;
            // 获取当前仓库的git提交信息
            var commitMessage = repo.Head.Tip.Message;

            return new GitRepositoryInfo(localPath, repositoryName, organization, branchName, commitTime.ToString(),
                commitAuthor, commitMessage, version);
        }
        else
        {
            if (string.IsNullOrEmpty(userName))
            {
                var str = Repository.Clone(repositoryUrl, localPath, cloneOptions);
            }
            else
            {
                var info = Directory.CreateDirectory(localPath);

                cloneOptions = new CloneOptions
                {
                    FetchOptions =
                    {
                        Depth = 0,
                        CertificateCheck = (certificate, chain, errors) => true,
                        CredentialsProvider = (_url, _user, _cred) =>
                            new UsernamePasswordCredentials
                            {
                                Username = userName, // 对于Token认证，Username可以随便填
                                Password = password
                            }
                    }
                };

                Repository.Clone(repositoryUrl, localPath, cloneOptions);
            }

            // 获取当前仓库的git分支
            using var repo = new Repository(localPath);
            var branchName = repo.Head.FriendlyName;
            // 获取当前仓库的git版本
            var version = repo.Head.Tip.Sha;
            // 获取当前仓库的git提交时间
            var commitTime = repo.Head.Tip.Committer.When;
            // 获取当前仓库的git提交人
            var commitAuthor = repo.Head.Tip.Committer.Name;
            // 获取当前仓库的git提交信息
            var commitMessage = repo.Head.Tip.Message;

            return new GitRepositoryInfo(localPath, repositoryName, organization, branchName, commitTime.ToString(),
                commitAuthor, commitMessage, version);
        }
    }
}