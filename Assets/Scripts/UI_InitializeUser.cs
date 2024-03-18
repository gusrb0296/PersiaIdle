using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_InitializeUser : UIBase
{
    [SerializeField] private Button touchToStart;
    [SerializeField] Button completeBtn;
    [SerializeField] Text warnText;
    [SerializeField] InputField inputField;
    [SerializeField] private GameObject[] inputUserNameUis;

    [SerializeField] private int minNameLength;
    [SerializeField] private int maxNameLength;

    [SerializeField] private string warnTextEmpty;
    [SerializeField] private string warnTextInvalid;
    [SerializeField] private string warnTextTooLongShort;

    // private GameManager gameManager;
    // private SceneManagerEx sceneManager;
    // private UIManager uiManager;

    private string inputName;
    // private UI_Alert alert;

    private bool isValid = false;

    private void Start()
    {
        Initialize();
        warnText.gameObject.SetActive(false);
    }

    private void Initialize()
    {
        // GetReferences();
        AddCallbacks();
    }

    // private void GetReferences()
    // {
    //     // gameManager = GameManager.Instance;
    //     // sceneManager = SceneManagerEx.Instance;
    //     // uiManager = UIManager.Instance;
    //
    //     // alert = uiManager.GetUIElement<UI_Alert>();
    // }

    private void AddCallbacks()
    {
        inputField.onValueChanged.AddListener(UpdateName);
        completeBtn.onClick.AddListener(CompleteBtnCallback);
        touchToStart.onClick.AddListener(CheckIDExist);
        // inputField.onValueChanged.AddListener((x)=>Debug.Log($"onValueChanged {x}"));
        // inputField.onSubmit.AddListener((x)=>Debug.Log($"onSubmit {x}"));
        // inputField.onEndEdit.AddListener((x)=>Debug.Log($"onEndEdit {x}"));
    }

    private void CheckIDExist()
    {
        if (ES3.KeyExists("userName"))
        {
            isValid = true;
            touchToStart.gameObject.SetActive(false);
            inputName = DataManager.Instance.Load<string>("userName");
            SceneChange();
        }
        else
        {
            // isValid = false;
            // touchToStart.gameObject.SetActive(false);
            // foreach (var ui in inputUserNameUis)
            //     ui.gameObject.SetActive(true);

            isValid = true;
            touchToStart.gameObject.SetActive(false);
            inputName = GenerateRandomId();
            SceneChange();
        }
    }


    public void UpdateName(string input)
    {
        inputName = input.Trim();
        // string playerName = inputName.Trim();

        StringBuilder warningMessage = new StringBuilder(); 

        // 빈 문자열인지 확인
        if (string.IsNullOrEmpty(inputName))
        {
            // Debug.LogError("닉네임을 입력해주세요.");
            warningMessage.Append(warnTextEmpty).Append("\n");
            // Alert(warnTextEmpty);
            // return;
        }

        // 길이 제한 확인
        // if (inputName.Length < minNameLength || inputName.Length > maxNameLength)
        // {
        //     // Debug.LogError($"닉네임은 {minNameLength}자 이상, {maxNameLength}자 이하로 설정해주세요.");
        //     warningMessage.Append(warnTextTooLongShort+"\n");
        //     // Alert(warnTextTooLongShort);
        //     // return;
        // }

        // 특수 문자나 공백이 포함되어 있는지 확인 (선택적)
        // if (inputName.Any(ch => !char.IsLetterOrDigit(ch)))
        // {
        //     // Debug.LogError("닉네임에는 특수 문자나 공백을 포함할 수 없습니다.");
        //     warningMessage.Append(warnTextInvalid);
        //     // return;
        // }

        if (warningMessage.Length > 0)
        {
            Alert(warningMessage.ToString());
            isValid = false;
        }
        else
        {
            warnText.gameObject.SetActive(false);
            isValid = true;
        }
        // if (inputName.Length > 0) warnText.gameObject.SetActive(false);
    }

    private void CompleteBtnCallback()
    {
        if (isValid) SceneChange();
        // if (inputName.Length > 1 && inputName.Length < 17) SceneChange();
        // else if (inputName.Length > 0) Alert(warnTextTooLongShort);
        // else Alert(warnTextEmpty);

        // inputField.text = "";
    }

    private void SceneChange()
    {
        // gameManager.SetNickName(inputName);
        GameManager.instance.SetNickName(inputName);
        CloseUI();
        SceneManager.LoadScene(1);
        // sceneManager.LoadScene(Enums.Scenes.GameScene);
    }
    
    private void Alert(string message)
    {
        warnText.text = message;
        warnText.gameObject.SetActive(true);
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
