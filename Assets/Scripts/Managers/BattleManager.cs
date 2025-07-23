using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BattleTestManager : MonoBehaviour
{
    [Header("테스트 설정")]
    public List<CookieData> testCookies;        // inspector에서 설정
    public BattleProcesser battleSimulator;

    [Header("시드")]
    public bool useCustomSeed = false;
    public int customSeed = 12345;

    void Start()
    {
        if (battleSimulator == null)
        {
            battleSimulator = FindObjectOfType<BattleProcesser>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartTestBattle();
        }
    }

    public void StartTestBattle()
    {
        if (testCookies == null || testCookies.Count < 2)
        {
            Debug.LogError("테스트용 쿠키 데이터가 최소 2개 필요합니다.");
            return;
        }

        int seed = GetBattleSeed();

        //var team1 = CreateBattleTeam(0, Mathf.Min(4, testCookies.Count));   // 0 ~ 9
        //var team2 = CreateBattleTeam(Mathf.Min(4, testCookies.Count), testCookies.Count);
        var team1 = CreateBattleTeam(0, 1);
        var team2 = CreateBattleTeam(2, 3);

        if (team2.Count == 0)
        {
            team2 = CreateBattleTeam(0, Mathf.Min(4, testCookies.Count));   // 0 ~ 9
        }

        battleSimulator.StartPVPBattle(team1, team2, seed);
    }

    private int GetBattleSeed()
    {
        if (useCustomSeed)
        {
            return customSeed;
        }
        else
        {
            int randomSeed = Random.Range(0, int.MaxValue);
            Debug.Log($"랜덤 시드값: {randomSeed}");
            return randomSeed;
        }
    }

    private List<IBattleUnit> CreateBattleTeam(int startIndex, int endIndex)
    {
        var team = new List<IBattleUnit>();

        for (int i = startIndex; i <= endIndex; i++)
        {
            if (i >= testCookies.Count)
            {
                Debug.LogWarning($"인덱스 {i}에 해당하는 쿠키 데이터가 없습니다.");
                continue;
            }

            var cookie = CreateCookie(testCookies[i], $"{testCookies[i].displayName}_{i}");
            team.Add(cookie);
        }

        return team;
    }

    private IBattleUnit CreateCookie(CookieData cookieData, string uniqueName)
    {
        GameObject cookieObject = Object.Instantiate(cookieData.prefab);
        var cookie = cookieObject.GetOrAddComponent<Cookie>();
        cookie.SetCookieData(cookieData);
        return cookie;
    }
}
