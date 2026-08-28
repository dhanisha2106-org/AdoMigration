namespace GraphQLBranchInfo;

public class BranchInfo
{
    public string? Name { get; set; }
    public CommitInfo? Target { get; set; }
}

public class CommitInfo
{
    public string? Oid { get; set; }
    public string? MessageHeadline { get; set; }
    public string? CommittedDate { get; set; }
    public AuthorInfo? Author { get; set; }
}

public class AuthorInfo
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class RepositoryResponse
{
    public Repository? Repository { get; set; }
}

public class Repository
{
    public BranchInfo? Ref { get; set; }
}

public class GraphQLResponse
{
    public RepositoryResponse? Data { get; set; }
}
