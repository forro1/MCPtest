# Phase 1 开发任务清单

## 目标

实现代际循环验证版本：

村庄出发 -> 不可靠地图探索 -> 卡牌战斗 -> 旅行者死亡或结束 -> 生成前任回声 -> 下一任旅行者找到回声 -> 选择立刻吸收或带回村庄研究 -> 村庄长期状态改变。

## 全局约束

- 不重写现有卡牌战斗系统。
- 不实现完整牌桌技能树。
- 不实现二阶段。
- 不实现真 Boss 和时间回溯剧情。
- 不引入重度村庄经营。
- 不决定最终存档介质，先实现内存态循环。
- 每个任务完成后都应能独立验证。

## 文件规划

### 新增目录

- `Assets/Scripts/GameFlow`
- `Assets/Scripts/Village`
- `Assets/Scripts/Traveler`
- `Assets/Scripts/Exploration`
- `Assets/Scripts/Legacy`
- `Assets/Scripts/Table`
- `Assets/Scripts/Battle`
- `Assets/Scripts/UI`

### 新增测试

- `Assets/Tests/Editor/GenerationalLoopTests.cs`
- `Assets/Tests/Editor/ExplorationMapTests.cs`
- `Assets/Tests/Editor/LegacyEchoTests.cs`
- `Assets/Tests/Editor/TableProgressTests.cs`

## Task 1: 村庄长期状态

**目标**：建立 Phase 1 的长期状态根对象。

**文件**：

- Create: `Assets/Scripts/Village/VillageState.cs`
- Create: `Assets/Scripts/Village/TravelerRecord.cs`
- Test: `Assets/Tests/Editor/GenerationalLoopTests.cs`

**需求**：

- `VillageState` 保存训练等级、旅行者记录、地图情报、前任回声、牌桌成长进度。
- 初始训练等级为 0。
- 能添加旅行者记录。
- 能添加前任回声。

**验收**：

- 测试能创建空村庄状态。
- 添加旅行者记录后数量增加。
- 添加前任回声后数量增加。

## Task 2: 当前旅行者生成

**目标**：根据村庄状态生成新任旅行者。

**文件**：

- Create: `Assets/Scripts/Traveler/TravelerRun.cs`
- Create: `Assets/Scripts/Traveler/TravelerFactory.cs`
- Test: `Assets/Tests/Editor/GenerationalLoopTests.cs`

**需求**：

- 新旅行者拥有编号、最大生命、当前生命、牌组标识、探索记录。
- 村庄训练等级应小幅提高最大生命。
- 生成旅行者时不修改村庄历史记录。

**验收**：

- 训练等级为 0 时生成基础生命旅行者。
- 训练等级提高后，新旅行者最大生命提高。
- 新旅行者当前生命等于最大生命。

## Task 3: 地图情报数据

**目标**：定义不可靠地图节点。

**文件**：

- Create: `Assets/Scripts/Exploration/MapNodeType.cs`
- Create: `Assets/Scripts/Exploration/MapIntelState.cs`
- Create: `Assets/Scripts/Exploration/MapNodeIntel.cs`
- Test: `Assets/Tests/Editor/ExplorationMapTests.cs`

**需求**：

- 节点包含显示类型、实际类型、风险等级、收益等级、可信度和来源。
- 节点可以标记是否允许节点误判、路线误差、风险收益误差。
- Phase 1 可信度使用整数 0-100。

**验收**：

- 可创建显示为事件、实际为战斗的节点。
- 可读取可信度和风险收益等级。
- 低可信度节点允许被标记为不可靠。

## Task 4: 最小探索地图

**目标**：生成一张小型探索地图。

**文件**：

- Create: `Assets/Scripts/Exploration/ExplorationMap.cs`
- Create: `Assets/Scripts/Exploration/ExplorationController.cs`
- Test: `Assets/Tests/Editor/ExplorationMapTests.cs`

**需求**：

- 地图包含 5-8 个节点。
- 至少包含普通战斗、事件、休息、前任回声、区域终点中的三类。
- 当前节点只能前进到可达节点。
- 首版可以使用固定模板加少量随机偏差。

**验收**：

- 地图生成后节点数量在 5-8。
- 起点存在。
- 至少一个节点可从起点到达。
- 节点进入后会产生节点结果。

## Task 5: 前任回声数据与生成

**目标**：旅行者死亡后生成前任回声。

**文件**：

- Create: `Assets/Scripts/Legacy/LegacyEcho.cs`
- Create: `Assets/Scripts/Legacy/LegacyEchoFactory.cs`
- Test: `Assets/Tests/Editor/LegacyEchoTests.cs`

**需求**：

- `LegacyEcho` 记录来源旅行者、区域线索、生成原因、奖励内容、是否已找回。
- `LegacyEchoFactory` 根据 `TravelerRun` 和死亡原因生成回声。
- 回声与死亡区域主题相关，但不要求精确坐标。

**验收**：

- 死亡旅行者能生成一个未找回回声。
- 回声包含来源旅行者编号。
- 回声包含区域线索。

## Task 6: 前任回声处理

**目标**：实现立刻吸收和带回村庄研究两个选择。

**文件**：

- Create: `Assets/Scripts/Legacy/LegacyEchoResolver.cs`
- Test: `Assets/Tests/Editor/LegacyEchoTests.cs`

**需求**：

- 立刻吸收影响当前旅行者。
- 带回村庄研究影响村庄长期状态。
- 处理后回声标记为已找回。

**验收**：

- 立刻吸收后当前旅行者获得当前局收益。
- 带回村庄研究后村庄训练等级或解锁状态改变。
- 同一个回声不能重复处理。

## Task 7: 牌桌能力最小入口

**目标**：为 Phase 1 提供一个轻量牌桌被动和一个主动技能入口。

**文件**：

- Create: `Assets/Scripts/Table/TableProgress.cs`
- Create: `Assets/Scripts/Table/TableAbility.cs`
- Create: `Assets/Scripts/Table/TableAbilityRuntime.cs`
- Test: `Assets/Tests/Editor/TableProgressTests.cs`

**需求**：

- `TableProgress` 保存理解阶段和已解锁能力。
- 支持一个被动能力标识。
- 支持一个主动技能标识。
- Phase 1 不实现完整技能树。

**验收**：

- 新村庄默认牌桌处于沉默阶段。
- 解锁一个被动后可查询。
- 解锁一个主动技能后可查询。

## Task 8: 战斗流程适配

**目标**：让探索节点能进入现有战斗，并返回战斗结果。

**文件**：

- Create: `Assets/Scripts/Battle/BattleRunRequest.cs`
- Create: `Assets/Scripts/Battle/BattleRunResult.cs`
- Create: `Assets/Scripts/Battle/BattleFlowAdapter.cs`
- Modify: `Assets/Scripts/SimpleCardBattle2D.Battle.cs`
- Test: `Assets/Tests/Editor/GenerationalLoopTests.cs`

**需求**：

- `BattleRunRequest` 描述敌人、玩家牌组、玩家生命和牌桌能力。
- `BattleRunResult` 描述胜利、失败、剩余生命、死亡原因。
- 适配层不重写 `TurnController`。

**验收**：

- 可构造一次战斗请求。
- 战斗结束后能得到胜负结果。
- 失败结果能进入死亡结算。

## Task 9: GameFlow 主流程

**目标**：串起村庄、新旅行者、地图、节点、战斗和死亡结算。

**文件**：

- Create: `Assets/Scripts/GameFlow/GamePhase.cs`
- Create: `Assets/Scripts/GameFlow/GameFlowController.cs`
- Test: `Assets/Tests/Editor/GenerationalLoopTests.cs`

**需求**：

- 可以开始新旅行者。
- 可以进入地图节点。
- 可以根据节点结果进入战斗或事件。
- 可以处理死亡并生成前任回声。
- 可以开始下一任旅行者。

**验收**：

- 从空村庄开始第一任旅行者。
- 第一任旅行者死亡后村庄出现旅行者记录和前任回声。
- 第二任旅行者生成后能看到前任回声线索。

## Task 10: 最小 UI

**目标**：提供 Phase 1 可手动验证的界面。

**文件**：

- Create: `Assets/Scripts/UI/VillageView.cs`
- Create: `Assets/Scripts/UI/ExplorationMapView.cs`
- Create: `Assets/Scripts/UI/LegacyEchoView.cs`
- Create: `Assets/Scripts/UI/RunSummaryView.cs`

**需求**：

- 村庄界面显示训练等级、旅行者数量、未找回回声数量。
- 地图界面显示节点情报。
- 回声界面提供立刻吸收和带回村庄研究按钮。
- 死亡摘要显示死亡原因和生成的回声。

**验收**：

- 玩家不用调试器也能完成两任旅行者流程。
- UI 能清楚显示当前局和长期状态变化。

## Task 11: 文档与验收记录

**目标**：让 Phase 1 开发过程可追踪。

**文件**：

- Modify: `docs/phase1/phase-1-generational-loop.md`
- Modify: `docs/phase1/phase-1-test-plan.md`

**需求**：

- 每完成一个任务，更新验收状态。
- 记录无法通过 Unity 自动化验证的内容。
- 记录任何偏离当前设计的实现决定。

**验收**：

- Phase 1 完成时，文档能说明哪些需求已实现、哪些延后、如何验证。

## 执行顺序

1. Task 1 村庄长期状态。
2. Task 2 当前旅行者生成。
3. Task 3 地图情报数据。
4. Task 4 最小探索地图。
5. Task 5 前任回声数据与生成。
6. Task 6 前任回声处理。
7. Task 7 牌桌能力最小入口。
8. Task 8 战斗流程适配。
9. Task 9 GameFlow 主流程。
10. Task 10 最小 UI。
11. Task 11 文档与验收记录。

## 开发前必须确认

正式编码前需要确认最终存档介质。如果未确认，Phase 1 只实现内存态循环。
