using UnityEngine;
// using Firebase;
// using Firebase.Extensions;
// using Firebase.Analytics;

public class DeviceIdChecking : MonoBehaviour
{
    // 테스트 기기들의 고유 ID 리스트
    private string[] testDeviceIds = { "f70fe41fb0676ca6a5f502abde7de006",
                                       "cOfe1516826c70f45a169f38a3ab2fcd",
                                       "c71d0c9e4ba81bf162d5e9c88c1aba92",
                                       "d57f06fe7ee09848dde7ea36f0eb97be",
                                       "c53e390c198ea335d5434c076b104df0",
                                       "654b99b1de9dee6125719a283b24d614",
                                       "109fcd783d2c3e3fa6febf10acb3f4b3", };

    void Start()
    {
        // 기기의 현재 고유 ID 가져오기
        string currentDeviceId = SystemInfo.deviceUniqueIdentifier;

        // 테스트 기기인지 체크
        // if (IsExcludedDevice(currentDeviceId))
        // {
        //     // Firebase 초기화 이벤트 로그 호출 막기
        //     FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        //         FirebaseApp app = FirebaseApp.DefaultInstance;
        //         // Firebase Analytics를 사용하지 않도록 설정
        //         FirebaseAnalytics.SetAnalyticsCollectionEnabled(false);
        //     });

        //     Debug.Log("테스트 기기입니다.");
        // }
    }

    // 특정 고유 ID를 가진 기기인지 확인
    bool IsExcludedDevice(string currentDeviceId)
    {
        return System.Array.Exists(testDeviceIds, id => id.Equals(currentDeviceId));
    }
}