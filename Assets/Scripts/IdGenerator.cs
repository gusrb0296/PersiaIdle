using UnityEngine;

public class IdGenerator : MonoBehaviour
{
    void Start()
    {
        // 로컬로 랜덤 ID 생성
        string randomId = GenerateRandomId();
        Debug.Log("Random ID: " + randomId);
    }

    string GenerateRandomId()
    {
        // 유저ID 생성
        string guid = System.Guid.NewGuid().ToString();
        string cleanedGuid = guid.Replace("-", "").ToLower();

        int desiredLength = 8;
        string randomId = cleanedGuid.Substring(0, Mathf.Min(cleanedGuid.Length, desiredLength));

        return "Userid_" + randomId;
    }
}