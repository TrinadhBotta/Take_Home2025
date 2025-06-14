using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

// Hashing utility class for SHA-256 and tagged hashing
public static class CryptoUtils
{
    public static byte[] Sha256(byte[] data)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(data);
    }

    public static byte[] TaggedHash(string tag, byte[] msg)
    {
        byte[] tagHash = Sha256(Encoding.UTF8.GetBytes(tag));
        byte[] combined = tagHash.Concat(tagHash).Concat(msg).ToArray();
        return Sha256(combined);
    }

    public static string ToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}



public class TaggedMerkleTree
{
    private string leafTag;
    private string branchTag;
    private readonly List<string> transactions;
    private readonly List<byte[]> leafHashes;
    private readonly List<List<byte[]>> levels = new();
    private readonly byte[] root;

    public TaggedMerkleTree(List<string> transactions, string leafTag, string branchTag)
    {
        this.transactions = transactions;
        this.leafTag = leafTag;
        this.branchTag = branchTag;
        this.leafHashes = transactions.Select(leaf => CryptoUtils.TaggedHash(leafTag, Encoding.UTF8.GetBytes(leaf))).ToList();
        this.levels.Add(new List<byte[]>(this.leafHashes));

        // Ensure the number of leaves is even and complete the last leaf if necessary
        if (this.levels[0].Count % 2 == 1)
            this.levels[0].Add(this.levels[0].Last());

        // Build the Merkle tree and compute the root
        this.root = BuildTree();
    }



    private byte[] BuildTree()
    {
        List<byte[]> currentLevel = new(this.leafHashes);

        while (currentLevel.Count > 1)
        {
            // If the current level has an odd number of hashes, duplicate the last one
            if (currentLevel.Count % 2 == 1)
                currentLevel.Add(currentLevel.Last());

            List<byte[]> nextLevel = new();

            // Combine pairs of hashes to create the parent level
            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                byte[] combined = currentLevel[i].Concat(currentLevel[i + 1]).ToArray();
                byte[] parent = CryptoUtils.TaggedHash(this.branchTag, combined);
                nextLevel.Add(parent);
            }

            this.levels.Add(nextLevel);
            currentLevel = nextLevel;
        }

        return currentLevel[0];
    }


    // Get the Merkle proof for a specific leaf index
    public List<(byte[], int)> GetProof(int index)
    {   
        if (index < 0 || index >= this.leafHashes.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the range of leaves.");
        
        List<(byte[], int)> proof = new();
        int idx = index;

        for (int levelIndex = 0; levelIndex < levels.Count - 1; levelIndex++)
        {
            if (idx % 2 == 0)
                proof.Add((levels[levelIndex][idx + 1],1));
            else
                proof.Add((levels[levelIndex][idx - 1],0));

            idx /= 2;
        }

        proof.Add((levels.Last()[0],0)); // Add the root hash as the last element in the proofss
        return proof;
    }

    public static bool VerifyProof(string leaf, List<(byte[], int)> proof, string leafTag, string branchTag)
    {   
        byte[] currentHash = CryptoUtils.TaggedHash(leafTag, Encoding.UTF8.GetBytes(leaf));

        // Exclude the last element (root) from the proof
        for (int i = 0; i < proof.Count - 1; i++){
            var (hash, direction) = proof[i];
            if (direction == 1){
            currentHash = CryptoUtils.TaggedHash(branchTag, currentHash.Concat(hash).ToArray());
            }
            else{
            currentHash = CryptoUtils.TaggedHash(branchTag, hash.Concat(currentHash).ToArray());
            }
        }

        var root = proof.Last().Item1; // Get the root from the last element of the proof
        return currentHash.SequenceEqual(root);
    }

    public string GetRootHex(){
        return BitConverter.ToString(this.root).Replace("-", "").ToLower();
    }

    public byte[] GetRoot(){
        return this.root;
    }
}