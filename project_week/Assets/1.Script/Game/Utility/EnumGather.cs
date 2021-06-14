using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    #region Game

    public enum SceneNum
    {
        BaseScene,
        LogoScene,
        LobbyScene,
        GameScene
    }

    public enum Windows
    {
        win_loadScene,
        win_message,
        win_purchase,
        win_celebrateEff,
        win_coinAni,
        win_accountList,
        win_serverLoad,

        max
    }

    public enum checker { none, ready, complete }
    public enum currency { coin, gem, ap }

    #endregion

    #region [ nanoo ]

    public enum nanooPost { coin, gem, skin, pack }
    public enum nanooPostMsg { key, message }

    #endregion

    #region [ firebase ]

    public enum analyticsWhere
    {
        store,
        post
    }

    #endregion

    #region [ user Data ]

    public enum mulCoinChkList
    {
        removeAD,
        mul_1st_10p,
        mul_1st_5p,
        mul_1st_3p,
        mul_2nd_3p,
        max
    }
    public enum paymentChkList
    {
        removeAD,
        mul_1st_10p,        
        startPack,
        skinPack,
        miniSet,
        santaSet,
        mul_1st_3p,
    }

    public enum utilityChkList
    {
        freeNickChange,         // 무료 닉변
        change_SecondStatus,    // 새 능력치로 변경되었는가 여부
        success_Recommend       // 리뷰 완료
    }

    #endregion
    
    #region dataTable

    public enum DataTable { status, skill, skin, monster, boss, enproj, quest, product, crest, obstacle, inquest, level, bulletData, max }
    
    /// <summary> 스탯 </summary>
    public enum StatusData
    {
        name,
        sName,

        origin,
        addition,
        additrate,

        bronze,
        silver,
        gold,
        platinum,

        cost,
        stair,

        max
    }

    public enum statusKeyList
    {
        hp,
        att,
        def,
        hpgen,
        cool,
        exp,
        coin,
        skin,
        max
    }
    public enum enemyStt { HP, ATT, DEF, SPEED, SIZE, EXP, COIN, max }

    /// <summary> 스킬 리스트 </summary>
    public enum SkillKeyList
    {
        HP,
        ATT,
        DEF,
        HPGEN,
        HEALMOUNT,
        COOL,
        SPEED,
        SIZE,
        BOSS,
        DODGE,
        EXP,
        COIN,

        SnowBall, 
        Icicle, FrostDrill,
        IceFist, IceKnuckle,
        HalfIcicle, SnowDart,
        Hammer,
        GigaDrill,
        Ricoche,

        IceBall, 
        Hail, Meteor,
        SinkHole, Crevasse,
        Circle,
        Poison, Lightning,
        Vespene, Thuncall,

        IceBat, 
        SnowBullet, SnowPoint,
        IceBalt, Recovery,
        Flurry, EyeOfFlurry,
        ColdStorm, RotateStorm,
        LockOn,

        Shield,
        HugeShield, GiantShield,
        ThornShield, ReflectShield, ChargeShield,
        Invincible, Absorb,
        Hide, Chill,

        Field,
        SnowStorm, Blizzard,
        SnowFog, WhiteOut, Aurora, 
        IceAge, SubStorm,

        Summon,
        Pet, Pet2, BPet,
        IceWall, IceBerg, Shard,
        Mine,

        Present,
        Dash,

        non
    }

    /// <summary> 스킬 값 </summary>
    public enum SkillValData
    {
        name,
        rank,
        skill_name,
        max_level,

        inheritType,
        essential,
        choice0,
        choice1,
        essential_Tem,

        shot,
        info,
        explain,        

        val0,        
        val0_increase,
        val0_cal, 
        val1,
        val1_increase,
        val1_cal,

        delay,
        delay_reduce,
        keep,
        keep_increase,
        size_increase,
        count,
        count_increase,
        range,

        eff,

        max
    }

    /// <summary> 스킨 종류 </summary>
    public enum SkinKeyList
    {
        snowman,

        fireman,
        grassman,
        rockman,
        citrusman,
        bulbman,
        wildman,
        mineman,
        robotman,
        icecreamman,
        goldman,
        angelman,
        squareman,
        spiderman,
        vampireman,
        heroman,
        santaman,
        presentman,
        dragonman,

        max
    }

    /// <summary> 스킨 값 </summary>
    public enum SkinValData
    {
        name,
        skinname,

        enable,

        currency,
        price,

        season,

        typeB,

        typeF,
        Fval0,
        Fval1,

        typeI,
        Ival,

        snowball,

        d_hp,
        d_att,
        d_def,
        d_hpgen,
        d_cool,
        d_exp,
        d_coin,
        d_speed, 

        ex_hp,
        ex_att,
        ex_def,
        ex_hpgen,
        ex_cool,
        ex_exp,
        ex_coin,

        max
    }

    public enum inheritType
    {
        rebase,
        non,
        over,
        union,
        medal,
        overmedal
    }

    public enum Mob
    {
        fire,
        closed,
        ranged,
        solid,
        ash,

        max
    }

    public enum MonsterData
    {
        name,
        hp,
        att,
        def,
        attspeed,
        speed,
        
        patt,
        pspeed,
        
        coin,
        exp,

        max,
        appear_term,
        
        spring,
        summer,
        fall,
        winter
    }

    public enum Boss
    {
        boss_bear,
        boss_butterfly,
        boss_flower,
        boss_scarecrow,
        boss_owl,
        all,
        max
    }

    public enum BossValData
    {
        hp,
        att,
        def,
        speed,
        skill0,
        skill1,
        coin,
        exp,
        reward,
        map,
        max
    }

    public enum InQuestKeyList
    {
        quest_0, quest_1, quest_2, quest_3, quest_4,
        quest_5, quest_6, quest_7, quest_8, quest_9,
        quest_10, quest_11, quest_12, quest_13, quest_14,
        quest_15, quest_16, quest_17, quest_18, quest_19,
        quest_20, quest_21, quest_22, quest_23,
        quest_24, quest_25, quest_26,
        max
    }

    public enum InQuestValData
    {
        title,
        explain,
        goal_Ex,
        goal_Key,
        goal_Condition,
        goal_condition_val,
        goal_val_type,
        goal_val,
        reward_Ex,
        reward_Key,
        reward_val_type,
        reward_val,
        exp,
        exchange,

        max
    }
    #region [quest]

    public enum inQuest_goal_key { kill, tem, dmg, time, skill }
    public enum gainableTem
    {
        heal, gem, exp, sward, present,
        questKey,
        //dropHam, dropBottle, dropLightPiece, dropBeard, dropMine,
        //hammerTem, poisonBottle, lightningTem, subShield, beardTem, mineTem,
        non
    } // 습득 가능한 아이템
    public enum inQuest_goal_valtype
    { 
        all, fire, close, range, hard, tree, quest,
        take, enemy, mob, boss,
        time, 
        cut, conti,
        non
    }

    public enum inQuest_goal_condition { under, snowball, skill, non }

    public enum inQuest_reward_key { get_stat, get_coin, get_skill, get_point, get_tem }
    public enum inQuest_reward_valtype { HP, ATT, DEF, HPGEN, COOL, EXP, HEALMOUNT, SPEED, coin, boss, skill, tem }

    #endregion

    public enum ShotList
    {
        SnowBall,
        Icicle,
        IceFist,
        HalfIcicle,
        FrostDrill,
        IceKnuckle,
        SnowDart,
        Hammer,
        GigaDrill,
        Ricoche,
        SnowBullet,
        SnowPoint,
        IceBalt,
        Thorn,
        Pet,
        Shard,

        IceBall,
        SinkHole,
        Crevasse,
        Poison,
        PoisonBomb,
        Mine,
        Present,

        None
    }

    public enum Stamp
    {
        iceballStamp,
        snowbombStamp,
        hailStamp,
        circleStamp,
        crevassStamp,
        meteorStamp,
        poisonStamp,
        lightningStamp,
        sulfurousStamp,
        thuncallStamp,

        icewallStamp,
        icebergStamp,
        mineStamp
    }

    public enum EnShot
    {
        fireball,
        lightning,
        banana,

        bfly_bgPoison,
        bfly_smPoison,
        flower_thorn,
        flower_mine,
        scarecrow_shot,
        scare_fire,
        owl_shot,
        bear_shot,

        max
    }

    public enum EnProjValData
    {
        name,
        speed,
        att,
        max
    }

    public enum Quest
    {
        day_rein, day_skin, day_revive, day_skill_0, day_skill_1, day_skill_2, day_max,

        rein, request,

        easy_time, normal_time, hard_time,
        easy_boss, normal_boss, hard_boss,
        max
    }

    public enum productKeyList
    {
        removead,
        startpack,

        vamppack,
        heropack,

        ad_gem,
        s_gem,
        m_gem,
        l_gem,
        b_gem,
        h_gem,        

        s_coin,
        m_coin,
        l_coin
    }

    public enum productValData
    {
        name, 
        disposable,
        pricetype, 
        price,
        removead,
        bonus,
        rate,
        gem, 
        ap, 
        coin, 
        addgem, 
        addap, 
        addcoin, 
        skin, 
        image
    }



    #region [recital Data]

    public enum snowballType { CitrusBall, SquareBall, RockBall, PresentBall, standard }
    public enum skinBvalue { light, wild, mine, invSlow, frozen, rebirth, blood, hero, present, max }
    public enum skinFvalue { light, wild, mine, iceHeal, rebirth, blood, present, max }
    public enum skinIvalue { mine, present, snowball, max }    

    #endregion

    #endregion

    #region gameData_sub

    public enum obstacleKeyList
    {
        dot_w_rock,
        h_w_rock,
        w_w_rock,
        L_w_rock,

        dot_n_rock,
        w_n_rock,

        dot_wood,
        dot_tree,

        dot_grass,
        w_grass,

        swamp0,
        swamp1,

        box_obstacle0,
        box_obstacle1,
        box_obstacle2,

        healpack,
        exppack,

        bosszone,

        ruin_thunder,
        ruin_drill,
        ruin_hammer,

        npc_house,

        dot_n_trap,

        max
    }

    public enum obsValList { hsize, wsize, mount, tem, easy, normal, hard }

    public enum levelKey { easy, normal, hard, max }
    public enum levelVal { mobrate, increase, coin, speed, trans, mapinfo, mobinfo, seasoninfo, season0, season1, season2, season3, boss, max }

    public enum lobbyPanel { levelPanel, nicPanel, optionPanel, postPanel, questPanel, rankPanel, recommendPanel, max }
    public enum effAni
    {
        attack,
        explosion,
        bossExplosion,
        hail,
        electric,
        mine,
        lava,
        crowfire,
        tel,
        playerhit,
        selfEx,
        storm
    }

    public enum dmgTxtType
    {
        standard,
        shield,
        heal,
        att,
        def,
        poison
    }

    #endregion

    public enum skillNote
    {
        none,
        pass,
        obstacle,
        continuee
    }

    #region 유저 데이터    

    #endregion

    public enum season { spring, summer, fall, winter, dark, max }

    #region [Sound]

    public enum BGM { Lobby, Battle, BattleHard, max }
    public enum SFX
    {
        click,

        purchase,

        randomStat,
        statup,

        coin, coin2,
        getTem,

        shot,

        inquest,
        inquestup,
        inquestClear,
        levelup,

        tree,
        icing,

        field,
        hail,
        meteor,
        crevasse,
        iceage,
        icebat,
        lightning,
        poison,
        shield,
        flurry,
        magic,
        magicend,
        iceberg,

        bossground,
        crowfire,
        crowshake,
        bossowl,
        owlfoot,
        bossTelpo,
        bossShot,
        bossdie,

        max
    }

    #endregion
}