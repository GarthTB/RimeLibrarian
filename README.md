# 词器清单版

一个用于维护Rime版星空键道6输入法词库的Windows工具

此工具是原版[词器](https://github.com/GarthTB/JDLibManager)的衍生版，所有功能集中在单个页面，砍掉了风险检查，流程更加通顺，一目了然。

同时，代码也比原版更加整洁且易于维护，所以后续基本不会再更新原版词器。

## 环境依赖

- [.NET 6.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0)

## 功能清单

- 加词：在词库中添加一个条目（有该词则词变红，有多码则码变红）
- 删除：在词库中删除一个条目
- 截短：将当前选中的词放在当前搜索的编码上，原本在这码上的词自动加长到剩下最短的空码上
- 应用修改：在列表中手动修改完成后，点击一下，将改动保存
- 日志：记录所有的改动，以便查错和回溯

## 注意

- 依赖官方Rime键道的词库分类法，需要一个cizu.yaml和一个danzi.yaml在相同目录中才能工作
- 仅在启动时加载一次词组和单字，关闭时写入，请确保妥善关闭
- 自动编码严格按照官方键道6的编码规则
- 词组保存时会按照编码升序来重排，可能破坏原有顺序
- 不支持某些生僻字（char类型无法容纳的，具体有哪些我也不清楚）

## 快捷键

- 主页F1：帮助
- 日志页F1：保存日志

---

# 更新日志

## [0.4.2] - 2024-08-31

- 修复：删除条目后不刷新的问题
- 优化：简化计算，去除冗余的变量和抽象类型以提高性能

## [0.4.1] - 2024-08-30

- 添加：词库里有该词时，字体变橙
- 优化：进一步并行化以提高性能
- 优化：完善错误处理逻辑

## [0.4.0] - 2024-08-30

- 优化：引入并行计算以进一步提高自动编码性能
- 优化：错误报告逻辑

## [0.3.1] - 2024-08-23

- 优化：提高自动编码性能
- 优化：帮助页不强制复制仓库链接

## [0.3.0] - 2024-07-20

- 修复：截短时短码词加长跳过原长码
- 优化：有多码可选时，字体变橙
- 优化：改变码长时，保持选中的编码
- 优化：优先级为0或留空时，不记录到日志中

## [0.2.0] - 2024-06-24

- 去除了修改页，直接在主页中修改！
- 优化操作逻辑，加词马上看得见！

## [0.1.0] - 2024-06-23

- 发布！