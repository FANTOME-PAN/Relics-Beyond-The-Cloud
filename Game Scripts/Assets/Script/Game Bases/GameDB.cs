using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameDB
{
    #region 单例 & 自启动
    private static GameDB _instance = new GameDB();
    public static GameDB Instance
    {
        get
        {
            return _instance = _instance ?? new GameDB();
        }
    }

    private GameDB()
    {
        Init();
    }

    private void Init()
    {
        funcCurvePath = Resources.Load<DataPathIndex>("Curve/A_DataPath");
        unitDataPath = Resources.Load<DataPathIndex>("Unit/A_DataPath");
        skillDataPath = Resources.Load<DataPathIndex>("Skill/A_DataPath");
        buffDataPath = Resources.Load<DataPathIndex>("Buff/A_DataPath");
    }
    #endregion

    #region 基础信息

    #region 游戏参数
    //最大速度加成上下限
    public const float MAX_BONUS_FOR_MAX_V = 1e7f;
    public const float MIN_BONUS_FOR_MAX_V = -1;
    //最大护盾值加成上下限
    public const float MAX_BONUS_FOR_MAX_SP = 1e7f;
    public const float MIN_BONUS_FOR_MAX_SP = -1;
    //最大转向速度加成上下限
    public const float MAX_BONUS_FOR_MAX_TURNING_V = 1e7f;
    public const float MIN_BONUS_FOR_MAX_TURNING_V = -1;
    //最大护盾值
    public const int MAX_SHEILD_POINT = (int)1e8;
    //最大魔法值
    public const float MAX_MANA_POINT = 1e7f;
    //最大速度
    public const int MAX_SPEED = (int)1e7;
    //最大转向速度
    public const float MAX_TURNING_V = (int)1e7;
    //拉升速度与最大转向速度比值
    public const float PULL_UP_CONST = 0.5f;
    public const float X_AXIS_BALANCING_CONST = 0.1f;
    //最大俯仰角
    public const float MAX_ROT_X = 75f;
    //最大俯仰角的倒数
    public const float RECIPROCAL_MAX_ROT_X = 1 / MAX_ROT_X;
    /// <summary>
    /// 单位移动阻尼
    /// </summary>
    public const float DAMPED_CONST = 2;
    /// <summary>
    /// 单位转向阻尼
    /// </summary>
    public const float ANGULAR_DAMPED_CONST = 3;
    #endregion

    #region 单位字典
    /// <summary>
    /// 单位池，记录所有单位信息
    /// </summary>
    public static UnitPool unitPool = new UnitPool();
    /// <summary>
    /// 投掷物池，记录所有投掷物信息
    /// </summary>
    public static MissilePool missilePool = new MissilePool();

    /// <summary>
    /// 将单位加入单位池(请勿调用，该方法不会触发单位出生事件，请使用Gamef.UnitBirth()方法)
    /// </summary>
    /// <param name="unitCtrl">单位控制组件</param>
    public void UnitBirth(UnitCtrl unitCtrl)
    {
        //如果单位已经设置过了，那么不执行
        if (unitCtrl.unit.ID != -1)
            return;
        UnitInfo unitInfo = new UnitInfo(unitCtrl);
        unitCtrl.unit.ID = unitPool.IDAlloc(unitInfo);
        //给UnitCtrl组件和其中的Unit类的unitInfo赋值，并且令Unit类初始化
        unitCtrl.unitInfo = unitInfo;
        unitCtrl.unit.Init(unitInfo);
    }
    /// <summary>
    /// 将投掷物加入池（请调用Gamef.MissileBirth）
    /// </summary>
    /// <param name="missile">投掷物</param>
    public void MissileBirth(Missile missile)
    {
        if (missile.ID != -1)
            return;
        missile.ID = missilePool.IDAlloc(missile);
    }
    /// <summary>
    /// 将投掷物清除（请调用Gamef.MissileClear）
    /// </summary>
    /// <param name="id">投掷物ID</param>
    public void MissileClear(int id)
    {
        missilePool.RemoveMissile(id);
    }

    /// <summary>
    /// 将单位移除单位字典(请勿调用，该方法不会触发单位清除事件，请使用Gamef.UnitClear()方法)
    /// </summary>
    /// <param name="unitID">单位ID</param>
    public void UnitClear(int unitID)
    {
        unitPool.RemoveUnit(unitID);
    }

    /// <summary>
    /// 通过ID获得单位信息
    /// </summary>
    /// <param name="unitID">单位ID</param>
    /// <returns>单位信息</returns>
    public UnitInfo GetUnit(int unitID)
    {
        return unitPool.GetUnit(unitID);
    }
    #endregion

    #region 场景序号
    /// <summary>
    /// 游戏中的各场景的序号
    /// </summary>
    public static class MyScene
    {
        public readonly static int LoadingScene = 0;
        public readonly static int GameScene = 1;
    }
    #endregion

    #region 预制Prefabs
    private GameObject _emptyObject;
    /// <summary>
    /// 空物体（仅有Transform组件）
    /// </summary>
    public GameObject EmptyObject
    {
        get
        {
            return _emptyObject = _emptyObject ?? Resources.Load<GameObject>("EmptyObject");
        }
    }
    private GameObject _bullet;
    public GameObject Bullet
    {
        get
        {
            return _bullet = _bullet ?? Resources.Load<GameObject>("Bullet");
        }
    }
    #endregion

    #region 曲线
    public static AnimationCurve[] curves = new AnimationCurve[10];
    public static AnimationCurve AtanCurve
    {
        get { return curves[(int)FuncCurveName.Arctan]; }
    }
    public static AnimationCurve SinCurve;
    private class Signal
    {
        public bool isBuild = false;
    }
    Signal signal = new Signal();

    public void BuildCurve()
    {
        lock (signal)
        {
            if (signal.isBuild)
            {
                // do nothing
            }
            else
            {
                BuildAtanCurve();
                BuildSinCurve();
                signal.isBuild = true;
            }
        }
    }

    private void BuildAtanCurve()
    {
        float tmp;
        Keyframe[] keyframes = new Keyframe[80];
        int cnt = 0;
        // 0 to 1 by 0.05
        for (int i = 0; i < 20; i++)
        {
            tmp = i * 0.05f;
            keyframes[cnt++] = new Keyframe(tmp, Mathf.Atan(tmp) * Mathf.Rad2Deg)
            {
                tangentMode = 1,
            };
        }
        // 1.1 to 3 by 0.1
        for (int i = 0; i < 20; i++)
        {
            tmp = 1.1f + i * 0.1f;
            keyframes[cnt++] = new Keyframe(tmp, Mathf.Atan(tmp) * Mathf.Rad2Deg)
            {
                tangentMode = 1,
            };
        }
        // 3.5 to 13 by 0.5
        for (int i = 0; i < 20; i++)
        {
            tmp = 3.5f + i * 0.5f;
            keyframes[cnt++] = new Keyframe(tmp, Mathf.Atan(tmp) * Mathf.Rad2Deg)
            {
                tangentMode = 1,
            };
        }
        // 14 to 24 by 1
        for (int i = 0; i < 10; i++)
        {
            tmp = 14f + i;
            keyframes[cnt++] = new Keyframe(tmp, Mathf.Atan(tmp) * Mathf.Rad2Deg)
            {
                tangentMode = 1,
            };
        }
        // 25 to ? by 10 i * i
        for (int i = 0; i < 10; i++)
        {
            tmp = 25f + 10f * i * i;
            keyframes[cnt++] = new Keyframe(tmp, Mathf.Atan(tmp) * Mathf.Rad2Deg)
            {
                tangentMode = 1,
            };
        }

        curves[(int)FuncCurveName.Arctan] = Resources.Load<FunctionCurve>(funcCurvePath.paths[(int)FuncCurveName.Arctan]).Curve = new AnimationCurve(keyframes);
    }

    private void BuildSinCurve()
    {
        Keyframe[] keyframes = new Keyframe[81];
        float tmp;
        for (int i = 0; i <= 80; i++)
        {
            tmp = 180f * (i * 0.025f - 0.5f);
            keyframes[i] = new Keyframe(tmp, Mathf.Sin(tmp * Mathf.Deg2Rad));
        }
        SinCurve = new AnimationCurve(keyframes)
        {
            postWrapMode = WrapMode.Loop,
            preWrapMode = WrapMode.Loop
        };
    }
    #endregion
    //函数曲线索引
    public DataPathIndex funcCurvePath;
    //单位数据索引
    public DataPathIndex unitDataPath;
    //技能数据索引
    public DataPathIndex skillDataPath;

    public DataPathIndex buffDataPath;

    public class SkillBuffer
    {
        public Skill skill;
        public SkillMgr.SkillEffectBase skillEffectBase;
    }
    public static SkillBuffer skillBuffer = new SkillBuffer();
    #endregion
}

public class UnitPool
{
    private const int BLK_LENGTH = 1 << 8;
    private struct InfoCell
    {
        public UnitInfo info;
        public bool isValid;
    }
    int maxLen = 0;
    List<InfoCell[]> blkList;
    List<UnitInfo> unitList = new List<UnitInfo>();
    public List<UnitInfo> UnitList
    {
        get
        {
            return unitList;
        }
    }
    //存储已经不被占用的id（即单位被清除）
    Queue<int> idQueue = new Queue<int>();
    /// <summary>
    /// 给单位分配ID
    /// </summary>
    /// <param name="info">目标单位</param>
    /// <returns>单位ID</returns>
    public int IDAlloc(UnitInfo info)
    {
        lock (blkList)
        {
            int res = -1;
            if (idQueue.Count > 0)
                res = idQueue.Dequeue();
            else
            {
                res = maxLen++;
                //如果超负荷，则申请一个新的数组
                if ((res & ~0xff) >= blkList.Count)
                {
                    InfoCell[] cells = new InfoCell[BLK_LENGTH];
                    for (int i = 0; i < BLK_LENGTH; i++)
                    {
                        cells[i].isValid = false;
                    }
                    blkList.Add(cells);
                }
            }
            unitList.Add((blkList[res & ~0xff][res & 0xff] = new InfoCell()
            {
                info = info,
                isValid = true,
            }).info);
            Debug.Log("Add unit to blk " + (res & ~0xff) + ", cell " + (res & 0xff));
            return res;

        }
    }
    /// <summary>
    /// 获取ID对应单位
    /// </summary>
    /// <param name="id">单位ID</param>
    /// <returns>单位</returns>
    public UnitInfo GetUnit(int id)
    {
        return blkList[id & ~0xff][id & 0xff].info;
    }
    /// <summary>
    /// 检查ID是否被占用
    /// </summary>
    /// <param name="id">单位ID</param>
    /// <returns>是否被占用</returns>
    public bool CheckID(int id)
    {
        return blkList[id & ~0xff][id & 0xff].isValid;
    }
    /// <summary>
    /// 移除单位
    /// </summary>
    /// <param name="id">单位ID</param>
    public void RemoveUnit(int id)
    {
        lock (blkList)
        {
            blkList[id & ~0xff][id & 0xff].isValid = false;
            unitList.Remove(blkList[id & ~0xff][id & 0xff].info);
            idQueue.Enqueue(id);
        }
    }

    public UnitPool()
    {
        InfoCell[] cells = new InfoCell[BLK_LENGTH];
        for (int i = 0; i < BLK_LENGTH; i++)
        {
            cells[i].isValid = false;
        }
        blkList = new List<InfoCell[]>()
        {
            cells,
        };
    }
}

public class MissilePool
{
    private const int BLK_LENGTH = 1 << 8;
    private struct InfoCell
    {
        public Missile info;
        public bool isValid;
    }
    int maxLen = 0;
    List<InfoCell[]> blkList;
    List<Missile> missileList = new List<Missile>();
    public List<Missile> MissileList
    {
        get
        {
            return missileList;
        }
    }
    //存储已经不被占用的id（即投掷物被清除）
    Queue<int> idQueue = new Queue<int>();
    /// <summary>
    /// 给投掷物分配ID
    /// </summary>
    /// <param name="missile">目标投掷物</param>
    /// <returns>投掷物ID</returns>
    public int IDAlloc(Missile missile)
    {
        lock (blkList)
        {
            int res = -1;
            if (idQueue.Count > 0)
                res = idQueue.Dequeue();
            else
            {
                res = maxLen++;
                //如果超负荷，则申请一个新的数组
                if ((res & ~0xff) >= blkList.Count)
                {
                    InfoCell[] cells = new InfoCell[BLK_LENGTH];
                    for (int i = 0; i < BLK_LENGTH; i++)
                    {
                        cells[i].isValid = false;
                    }
                    blkList.Add(cells);
                }
            }
            missileList.Add((blkList[res & ~0xff][res & 0xff] = new InfoCell()
            {
                info = missile,
                isValid = true,
            }).info);
            Debug.Log("Add missile to blk " + (res & ~0xff) + ", cell " + (res & 0xff));
            return res;
        }
    }
    /// <summary>
    /// 获取ID对应投掷物
    /// </summary>
    /// <param name="id">投掷物ID</param>
    /// <returns>投掷物</returns>
    public Missile GetMissile(int id)
    {
        return blkList[id & ~0xff][id & 0xff].info;
    }
    /// <summary>
    /// 检查ID是否被占用
    /// </summary>
    /// <param name="id">投掷物ID</param>
    /// <returns>是否被占用</returns>
    public bool CheckID(int id)
    {
        return blkList[id & ~0xff][id & 0xff].isValid;
    }
    /// <summary>
    /// 移除投掷物
    /// </summary>
    /// <param name="id">投掷物ID</param>
    public void RemoveMissile(int id)
    {
        lock (blkList)
        {
            blkList[id & ~0xff][id & 0xff].isValid = false;
            missileList.Remove(blkList[id & ~0xff][id & 0xff].info);
            idQueue.Enqueue(id);
        }
    }

    public MissilePool()
    {
        InfoCell[] cells = new InfoCell[BLK_LENGTH];
        for (int i = 0; i < BLK_LENGTH; i++)
        {
            cells[i].isValid = false;
        }
        blkList = new List<InfoCell[]>()
        {
            cells,
        };
    }
}