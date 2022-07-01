
using System;
using System.Runtime;
using System.Collections.Generic;


namespace GersonFrame
{
    public enum MsgType
    {
        None,
        ConnectServerFail,
        /// <summary>
        /// 下一波次动画开始
        /// </summary>
        OnNextWaveAmStart,
        /// <summary>
        /// 下一波次动画结束
        /// </summary>
        OnNextWaveAmEnd,

        /// <summary>
        /// 敌人Boss死亡
        /// </summary>
        OnEnemyBossDeath,

        /// <summary>
        /// 连击
        /// </summary>
        OnHit,
        /// <summary>
        /// 当玩家复活时
        /// </summary>
        OnRevive,
        /// <summary>
        /// 玩家角色死亡
        /// </summary>
        OnPlayerDeath,
        /// <summary>
        /// 当改变金币的接收方
        /// </summary>
        OnChangeAttackTs,
        /// <summary>
        /// 当玩家获取钻石时
        /// </summary>
        OnGetDiamon,
        /// <summary>
        /// 当获取动画结束
        /// </summary>
        OnGetAnimEnd,
        /// <summary>
        /// 当金币或钻石数量发生变化
        /// </summary>
        OnRichesChanged,
        /// <summary>
        /// 当金币或钻石不足时
        /// param 金币：true，钻石：false
        /// </summary>
        OnRichesNotEnough,
        /// <summary>
        /// 当体力使用或回复时
        /// </summary>
        OnStrengthChanged,
        /// <summary>
        /// 当体力不足时
        /// </summary>
        OnStrengthNotEnough,



        /// <summary>
        /// 当商店展示模型改变时
        /// </summary>
        OnChangeShowMod,
        /// <summary>
        /// 当改变出战角色时
        /// </summary>
        OnChangeCurSelectRole,

        /// <summary>
        /// 当显示结算奖励动画时
        /// </summary>
        OnPlayRewardAnim,
        /// <summary>
        /// 当英雄升级时
        /// </summary>
        OnUpgradeHero,
        /// <summary>
        /// 当切换神器界面时
        /// </summary>
        OnChangeGrail,
        /// <summary>
        /// 神器升级结束时刷新界面
        /// </summary>
        OnGrailUpgradeEnd,

        /// <summary>
        /// 当显示天赋提示时
        /// </summary>
        OnShowGiftTips,
        /// <summary>
        /// 当显示神器详细信息时
        /// </summary>
        OnShowGrailTips,
        /// <summary>
        /// 当购买体力时
        /// </summary>
        OnBuyStrength,
        /// <summary>
        /// 当选择挂机英雄时
        /// </summary>
        OnSelectHangUpHero,
        /// <summary>
        /// 播放活跃度获取动画
        /// </summary>
        ActivationAnim,
        /// <summary>
        /// 当玩家更换名字时
        /// </summary>
        OnNameChanged,
        /// <summary>
        /// 显示成就界面的装备信息
        /// </summary>
        ShowEquipmentInfoA,
        /// <summary>
        /// 显示装备魔盒的装备信息
        /// </summary>
        ShowSelectedInfoM,
        /// <summary>
        /// 当更换&装备 装备时
        /// </summary>
        OnChangeEquipment,
        /// <summary>
        /// 更改神器技能释放者
        /// </summary>
        ChangeGrailSkillPlayer,
        /// <summary>
        /// 当任务刷新时
        /// </summary>
        OnRefreshTask,
        /// <summary>
        /// 刷新挂机速率
        /// </summary>
        RefreshHangupRate,
        /// <summary>
        /// 获取成就奖励
        /// </summary>
        OnGetAchievement,
        /// <summary>
        /// 选择被分解的装备
        /// </summary>
        OnSelectDecompositionEquipment,
        /// <summary>
        /// 取消选择分解装备
        /// </summary>
        OnDeselectDecompositionEquipment,
        /// <summary>
        /// 任务跳转
        /// </summary>
        TaskJump,
        /// <summary>
        /// 解锁功能动画
        /// </summary>
        UnlockFunctionAnim,
        /// <summary>
        /// 获取英雄动画
        /// </summary>
        GetHeroAnim,
        /// <summary>
        /// 滑动切换神器
        /// </summary>
        ChangeGrailRoll,
        /// <summary>
        /// 家园界面刷新资源数量
        /// </summary>
        RefreshResource,
        /// <summary>
        /// 守护者大厅获得的体力
        /// </summary>
        GuradianStrength,
        /// <summary>
        /// 交易所获得金币
        /// </summary>
        ExchangeCoin,
        /// <summary>
        /// 奖励获得界面展示tips
        /// </summary>
        RewardPanelTips,
        /// <summary>
        /// 改变家园进驻英雄
        /// </summary>
        HomelandChangeHero,
        /// <summary>
        /// 装备魔盒选择装备
        /// </summary>
        MagicBoxSelectEquipment,
        /// <summary>
        /// 重铸界面点击锁定
        /// </summary>
        RecastLock,
        /// <summary>
        /// 选择镶嵌宝石
        /// </summary>
        SelectInlayGem,
        /// <summary>
        /// 镶嵌宝石
        /// </summary>
        GemInlay,
        /// <summary>
        /// 选择升星材料
        /// </summary>
        SelectCostEquip,
        /// <summary>
        /// 装备升星时
        /// </summary>
        OnEquipmentLevelUp,
        /// <summary>
        /// 选择合成宝石
        /// </summary>
        SelectTargetGem,
        /// <summary>
        /// 获取装备时
        /// </summary>
        GetEquipment,
        /// <summary>
        /// 家园建筑升级
        /// </summary>
        HomelandBuildingUpgrade,
        /// <summary>
        /// 装备魔盒显示宝石详情
        /// </summary>
        ShowGemInfo,
        /// <summary>
        /// 击杀小怪
        /// </summary>
        KillMonster,
        /// <summary>
        /// 击杀boss
        /// </summary>
        KillBoss,
        /// <summary>
        /// 新手引导获取点击区域位置和功能
        /// </summary>
        GuideGetPositionFunction,
        /// <summary>
        /// 解锁新功能
        /// </summary>
        UnlockNewFunction,
        /// <summary>
        /// 改变难度
        /// </summary>
        ChangeHard,

        /// <summary>
        /// 退出登陆时
        /// </summary>
        OnLogoutSuccess,
        /// <summary>
        /// 刷新好友列表
        /// </summary>
        RefreshFriendLists,
        /// <summary>
        /// 好友新消息
        /// </summary>
        FriendNewChat,
        /// <summary>
        /// 好友请求提示
        /// </summary>
        NotifyAddFriend,
        /// <summary>
        /// 被删除好友提示
        /// </summary>
        NotifyDelFriend,
        /// <summary>
        /// 好友请求同意提示
        /// </summary>
        NotifyAgreeFriend,
        /// <summary>
        /// 当变更头像
        /// </summary>
        OnHeadIconChange,
        /// <summary>
        /// 当变更头像框
        /// </summary>
        OnHeadIconEdgeChange,
        /// <summary>
        /// 改变主界面页面
        /// </summary>
        ChangeMainPanelPage,

        HangUpShakeRewardImage

    }


}