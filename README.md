## 来源
魔改了一下[UnityUIPolygon](https://github.com/SentientDragon5/UnityUIPolygon/tree/main)，使其可以结合Animation使用。
### 原理
通过将多边形的顶点信息存储到子节点中，然后通过Animation存储子节点信息，运行时将子节点的信息读取出来更新到顶点信息列表中。
### 使用方法
1. 在Hierarchy里右键->UI->Polygon，创建新多边形
2. 在Polygon组件上拖个Texture，然后点击SetNativeSize
3. 勾选canEdit，然后添加任意个顶点，并在Scene场景中调整其位置
4. 调整好顶点位置后，点击UpdateChildren将顶点信息存到子节点中
5. 如果不勾选canEdit，可以在编辑器里的Animation窗口预览动画，但是就不能编辑顶点了
6. 在编辑动画时，建议先UpdateChildren再Add Key
### 参考项目
