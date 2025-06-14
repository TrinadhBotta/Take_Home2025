using System;

public class TestMerkleTree
{
    public static void runTest()
    {
        List<string> transactions = new()
        {
            "aaa",
            "bbb",
            "ccc",
            "ddd",
            "eee"
        };

        var leafTag = "Bitcoin_Transaction";
        var branchTag = "Bitcoin_Transaction";

        TaggedMerkleTree merkleTree = new(transactions, leafTag , branchTag);
        Console.WriteLine("Merkle Root: " + merkleTree.GetRootHex());
       

        int indexToVerify = 1;
        List<(byte[], int)> proof = merkleTree.GetProof(indexToVerify);

        //check if proof is valid for the transacation at index 1 as "bbb"
        bool isValid = TaggedMerkleTree.VerifyProof("bbb", proof, leafTag, branchTag);
        Console.WriteLine($"Proof for transaction at index {indexToVerify} and transaction {"bbb"} is : {isValid}");

        //check if proof is valid for the transacation at index 1 as "aaa"
        isValid = TaggedMerkleTree.VerifyProof("aaa", proof, leafTag, branchTag);
        Console.WriteLine($"Proof for transaction at index {indexToVerify} and transaction {"aaa"} is : {isValid}");


    }
}