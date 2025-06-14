using MYAPP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


[ApiController]
[Route("api/[controller]")]

public class MerkleController : ControllerBase
{   
    private readonly MerkleTreeService _merkleService;

    public MerkleController(MerkleTreeService merkleService)
    {
        _merkleService = merkleService;
    }
    
    // Serialize (User ID, Balaance) as a string tuple "(userId, balance)"
    public static string SerializeTransaction(int userId, int balance)
    {
        return $"({userId},{balance})";
    }

    [HttpGet("createExampleMerkleTree")]
    public IActionResult CreateExampleMerkleTree()
    {
        _merkleService.Transactions.Clear();
        _merkleService.TransactionsMap.Clear();

        for (int i = 1; i < 9; i++)
        {
            var serialized = SerializeTransaction(i, i * 1111);
            _merkleService.Transactions.Add(serialized);
            _merkleService.TransactionsMap[i] = i * 1111;
        }

        _merkleService.MerkleTree = new TaggedMerkleTree(
            _merkleService.Transactions,
            _merkleService.LeafTag,
            _merkleService.BranchTag
        );

        return Ok(new { MerkleRoot = _merkleService.MerkleTree.GetRootHex() });
    }

    [HttpGet("getMerkelProof")]
    public IActionResult GetMerkleProof([FromQuery(Name = "userId")] string userId)
    {
        if (_merkleService.MerkleTree == null || _merkleService.TransactionsMap.Count == 0)
        {
            return BadRequest("Merkle tree has not been created. Please create it first.");
        }

        if (!int.TryParse(userId, out int uid) || !_merkleService.TransactionsMap.TryGetValue(uid, out int userBalance))
        {
            return NotFound($"User ID {userId} not found.");
        }

        var merkleProofRoute = _merkleService.MerkleTree.GetProof(uid - 1);
        var tempMerkleProof = new List<MerkleProofEntry>();

        foreach (var proof in merkleProofRoute)
        {
            var hexProof = CryptoUtils.ToHex(proof.Item1);
            tempMerkleProof.Add(new MerkleProofEntry
            {
                Hash = hexProof,
                Direction = proof.Item2
            });
        }

        var returMerkleProof = new {
            UserBalance = userBalance,
            Proof = tempMerkleProof
        };

        return Ok(returMerkleProof);
    }
}