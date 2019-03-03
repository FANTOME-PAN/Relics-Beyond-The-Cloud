using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameCtrl : MonoBehaviour
{
    #region 单例
    /// <summary>
    /// 游戏控制器单例
    /// </summary>
    public static GameCtrl Instance { get; private set; }
    /// <summary>
    /// 游戏控制组件是否初始化完成。在Controller的Start执行后为真。
    /// </summary>
    public static bool isInit = false;
    #endregion

    #region 实时公有信息
    //private UnitInfo _mainChara;
    public static UnitCtrl PlayerUnit;
    public UnitInfo PlayerChara
    {
        get
        {
            return PlayerUnit.unitInfo;
        }
        set
        {
            PlayerUnit = value.UnitCtrl;
        }
    }

    public Transform PlayerCamera
    {
        get; set;
    }
    #endregion

    public bool BuildSkillDataPath = false;

    #region 生命周期
    private void Awake()
    {
        Instance = this;
        EventMgr.initEvent.OnAwake();
        EventMgr.UpdateEvent.AddListener(InputMgr.CheckHotKey);
        InputMgr.BindHotKey(TestHotKey, KeyCode.F);
        InputMgr.BindHotKey(TestCasting, KeyCode.T);
        BindHotKey4Skill();
    }

    private void Start()
    {
        EventMgr.initEvent.OnStart();
        DontDestroyOnLoad(gameObject);
        //GameDB.Instance.BuildCurve();
        SkillMgr.InitSkills();
        if (BuildSkillDataPath)
            SkillMgr.BuildSkillDataPath();
        isInit = true;//初始化完毕
        //加载游戏场景
        SceneManager.LoadSceneAsync(GameDB.MyScene.GameScene);
    }

    public AnimationCurve AtanCurve;
    public AnimationCurve tempCurve;
    int cnt = 0;
    float avgRate = 0;
    private void TestCasting()
    {
        int len = 10000000;
        long tt1, tt2;
        object obj = new TestClass() { a = 1 };
        TestClass test = obj as TestClass;
        CHLDClass chld;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        float t3 = 3f;
        for (int i = 0; i < len; i++)
        {

        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(tt1 = stopwatch.ElapsedMilliseconds);
        float a = 21.312f, b = 0.1f;
        stopwatch.Reset();
        stopwatch.Start();
        for (int i = 0; i < len; i++)
        {
            a = a * b;
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(tt2 = stopwatch.ElapsedMilliseconds);
        avgRate += tt1 / (float)tt2;
        cnt++;
        UnityEngine.Debug.Log("Avg Rate is " + avgRate / cnt);
        //InputMgr.UnbindHotKey(TestCasting, KeyCode.T);
    }

    private void Update()
    {
        EventMgr.UpdateEvent.OnTrigger();
    }

    [SerializeField]
    private UnitCtrl player;
    private void TestHotKey()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitCtrl>();
        StartCoroutine(Dartle(5, 0.2f));
        UnityEngine.Debug.Log("按下了快捷键！！！");
    }

    private IEnumerator Dartle(int times, float dur)
    {
        for (int i = 0; i < times; i++)
        {
            Instantiate(GameDB.Instance.Bullet, player.BulletSpawnPoint.position, Quaternion.LookRotation(player.BulletSpawnPoint.forward, Vector3.up));
            yield return new WaitForSeconds(dur);
        }
    }

    #endregion

}

/// <summary>
/// 单位的全面信息
/// </summary>
public class UnitInfo
{
    public UnitCtrl UnitCtrl { get; private set; }
    public GameObject Obj { get; private set; }
    public Transform Transform { get; private set; }
    /// <summary>
    /// 构造单位全面信息
    /// </summary>
    /// <param name="unitCtrl">单位控制组件</param>
    public UnitInfo(UnitCtrl unitCtrl)
    {
        UnitCtrl = unitCtrl;
        Obj = unitCtrl.gameObject;
        Transform = Obj.transform;
    }
}

/// <summary>
/// 护甲类型
/// </summary>
public enum ArmorType
{
    //无敌的
    invincible = 0,
    //

    //

    //
}

/// <summary>
/// 伤害类型
/// </summary>
public enum DamageType
{
    unset,
}

public class TestClass
{
    public int a;
    public float b;
    public string c;
    private long d;
    public long D
    {
        get
        {
            return d << 3;
        }
        set
        {
            d = value >> 3;
        }
    }
}

public class CHLDClass : TestClass
{
    public int k;
    public Vector3 m;
}