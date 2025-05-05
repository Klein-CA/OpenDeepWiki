namespace KoalaWiki.Git;

/// <summary>
/// Git仓库信息记录类，用于存储与Git仓库相关的详细信息。
/// </summary>
/// <param name="LocalPath">本地路径，表示Git仓库在本地文件系统中的位置。</param>
/// <param name="RepositoryName">仓库名称，表示Git仓库的名称。</param>
/// <param name="Organization">组织名称，表示Git仓库所属的组织或用户。</param>
/// <param name="BranchName">分支名称，表示当前Git仓库所在的分支。</param>
/// <param name="CommitTime">提交时间，表示最后一次提交的时间。</param>
/// <param name="CommitAuthor">提交作者，表示最后一次提交的作者。</param>
/// <param name="CommitMessage">提交信息，表示最后一次提交的提交信息。</param>
/// <param name="Version">版本信息，表示Git仓库的当前版本。</param>
public record GitRepositoryInfo(
    string LocalPath,
    string RepositoryName,
    string Organization,
    string BranchName,
    string CommitTime,
    string CommitAuthor,
    string CommitMessage,
    string Version);