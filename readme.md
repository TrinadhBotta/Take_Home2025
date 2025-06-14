## Merkle Tree API

### 1. **Create Merkle Tree**

* **Endpoint:** `GET /api/merkle/createExampleMerkleTree`
* **Description:**

  Creates a sample Merkle Tree using 8 users.

  Each user has a balance = `userId * 1111`.

  Returns the Merkle root.
* Response:

  ```
  {
    "merkleRoot": "b1231de33da17c23cebd80c104b88198e0914b0463d0e14db163605b904a7ba3"
  }
  ```

### 2. **Get Merkle Proof**

* **Endpoint:** `GET /api/merkle/getMerkelProof?userId=3`
* **Description:**

  Returns the Merkle proof for a given user ID.

  You must first call the create tree API.
* **Query Parameter:**

  * `userId` (e.g., 3)
* **Response:**

```
{
  "userBalance": 3333,
  "proof": [
    {
      "hash": "8520072399ad3462db395a7a9803c6fe3f4143d502a0eb145e6c69ba7ec6d22d",
      "direction": 1
    },
    {
      "hash": "99fb04c9b8fd37e66b2dde367d91f2c930b2ab162dbbf7298e9313c309c7925f",
      "direction": 0
    },
    {
      "hash": "9d7f79fa8e788d4a32c9c674b67dcfaf0885f539ac2699129e3c4d88c11c76e7",
      "direction": 1
    },
    {
      "hash": "b1231de33da17c23cebd80c104b88198e0914b0463d0e14db163605b904a7ba3",
      "direction": 0
    }
  ]
}
```


## TestMerkleTree

This test checks how a Merkle Tree with tagged hashing works using example transaction data.

### What the test does

1. A list of transactions is created:

   `["aaa", "bbb", "ccc", "ddd", "eee"]`
2. A Merkle Tree is built using this list.
3. The Merkle root is printed.
4. A Merkle proof is created for the transaction at index 1, which is `"bbb"`.
5. Two checks are done to verify the proof.

---

### Test Case 1

**What it tests:**

Verifies the proof using the correct transaction `"bbb"`.

**Result:**

True — the proof is valid for `"bbb"`.

---

### Test Case 2

**What it tests:**

Verifies the same proof using a wrong transaction `"aaa"`.

**Result:**

False — the proof is not valid for `"aaa"`.

This API provides two endpoints to work with a Merkle Tree. It helps create a Merkle root and generate Merkle proofs for users.

---
