using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json; // JSON 직렬화/역직렬화를 위한 Newtonsoft.Json
using Newtonsoft.Json.Converters;   // string 형식을 enum으로 변환할 수 있게 해줌
using System.IO;       // 파일 입출력
using System.Linq;     // LINQ 사용

// - 게임 데이터 구조 (예: 아이템, 캐릭터 등)
[System.Serializable]
public class GameData
{
    public List<CharacterData> character_table; // 캐릭터 데이터 테이블
    public List<SkillData> skill_table;         // 스킬 데이터 테이블
}

// - 열거형 데이터 사전 선언
public enum Sk_EffectType { missile, buff, instant }

// - 캐릭터 데이터 클래스
[System.Serializable]
public class CharacterData
{
    public int character_id;    // 고유 ID
    public string name;         // 이름 호출 코드
    public string description;  // 설명 호출 코드
    public float ch_HP_base;    // 기본 체력
    public float ch_ATK_base;   // 공격력
    public int ch_PhyATK_base;  // 물리 공격력
    public int ch_EnATK_base;   // 에너지 공격력
    public float ch_CRI_base;   // 치명타 확률
    public float ch_CATK_base;  // 치명타 배율
    public float ch_DEX_base;   // 공격속도
    public float ch_MeleeRange_base;    // 근접공격 범위
    public int ch_ARM_base;             // 방어구
    public float ch_SPD_base;           // 이동속도 증가량
    public float ch_PickupRange_base;   // 획득 범위
    public float ch_Mining_base;        // 채굴 증가량
}

[System.Serializable]
public class SkillData
{
    public int skill_id;                        // 스킬 ID
    public float sk_Cooldown_base;              // 쿨타임
    public int sk_Repeat_base;                  // 연속사용 횟수
    public float sk_RepeatCooldown_base;        // 연속사용 대기시간
    public int sk_MaxCharges_base;              // 최대 충전량
    public bool sk_ChargeAll;                   // 쿨타임이 돌았을때 충전량이 전부 회복되는지 여부
    [JsonConverter(typeof(StringEnumConverter))]
    public Sk_EffectType sk_EffectType;         // 스킬 사용시의 효과 종류 (예 : 투사체, 버프, 특수 )
    public int sk_Effect_id;                    // 발동시킬 효과의 ID
}

// - 저장용 플레이어 데이터
[System.Serializable]
public class SaveData
{
    public string lastSaveTime; // 마지막 저장 시각
    public int playerLevel;     // 플레이어 레벨
    public int gold;            // 보유 골드
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance; // 싱글턴 인스턴스

    private Dictionary<int, CharacterData> characterDict = new(); // 로드된 데이터를 저장하는 딕셔너리
    private Dictionary<int, SkillData> skillDict = new(); // 로드된 데이터를 저장하는 딕셔너리

    public static SaveData currentSaveData; // 현재 세이브 데이터

    // 저장 경로: 기기별 고유 폴더에 저장
    public static string savePath => Application.persistentDataPath + "/save.json";

    // - 씬이 시작될 때 호출됨
    void Awake()
    {
        // 싱글턴 패턴 적용: 인스턴스가 없다면 자신을 등록
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
            LoadData();  // JSON 데이터 테이블 로드
            LoadSave();  // 세이브 파일 로드
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있다면 제거
        }
    }

    // - Resources 폴더의 JSON 데이터를 불러와 딕셔너리에 저장
    void LoadData()
    {   
        // Resources 폴더의 JSON 파일 읽어오기
        TextAsset jsonFile = Resources.Load<TextAsset>("data_table"); // Resources/data_table.json
        if (jsonFile == null)
        {
            Debug.LogError("data_table.json 파일을 찾을 수 없습니다!");
            return;
        }

        // JSON을 GameData 형식으로 변환
        GameData data = JsonConvert.DeserializeObject<GameData>(jsonFile.text); // JSON → 객체 변환

        // 각 테이블에 있는 데이터를 딕셔너리에 저장
        foreach (var character in data.character_table)
            characterDict[character.character_id] = character;
        foreach (var skill in data.skill_table)
            skillDict[skill.skill_id] = skill;

        Debug.Log($"캐릭터 데이터 {characterDict.Count}개 로드 완료!");
        Debug.Log($"스킬 데이터 {skillDict.Count}개 로드 완료!");
    }

    // - 특정 ID의 데이터를 가져옴
    public CharacterData GetCharacterData(int id)       { return characterDict.TryGetValue(id, out var data) ? data : null; }
    public SkillData GetSkillData(int id)               { return skillDict.TryGetValue(id, out var data) ? data : null; }

    // - 모든 데이터를 리스트 형태로 반환
    public List<CharacterData> GetAllCharacterData()    { return characterDict.Values.ToList(); }
    public List<SkillData> GetAllSkillData()            { return skillDict.Values.ToList(); }

    // - 세이브 데이터를 파일로 저장
    public void SaveGame()
    {
        if (currentSaveData == null) return;

        // 현재 시간으로 마지막 저장 시각 갱신
        currentSaveData.lastSaveTime = System.DateTime.Now.ToString();

        // JSON으로 직렬화 (Newtonsoft.Json.Formatting 사용)
        string json = JsonConvert.SerializeObject(
            currentSaveData,
            Newtonsoft.Json.Formatting.Indented // 보기 좋은 포맷
        );

        File.WriteAllText(savePath, json); // 파일 저장
        Debug.Log("세이브 저장 완료");
    }

    // - 세이브 파일이 존재하면 로드하고, 없으면 새로 생성
    public void LoadSave()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonConvert.DeserializeObject<SaveData>(json);
            Debug.Log("세이브 불러오기 완료");
        }
        else
        {
            Debug.Log("세이브 없음 → 새로 생성");
            currentSaveData = new SaveData
            {
                lastSaveTime = System.DateTime.Now.ToString(),
                playerLevel = 1,
                gold = 1000
            };
            SaveGame(); // 즉시 저장
        }

        Debug.Log("세이브 경로: " + savePath);
    }
}