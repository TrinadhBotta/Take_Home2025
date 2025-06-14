public class MerkleTreeService
{
    public List<string> Transactions { get; set; } = new();
    public Dictionary<int, int> TransactionsMap { get; set; } = new();
    public TaggedMerkleTree MerkleTree { get; set; }
    public string LeafTag { get; } = "ProofOfReserve_Leaf";
    public string BranchTag { get; } = "ProofOfReserve_Branch";
}