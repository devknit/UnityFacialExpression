# ZanFacialExpression

SkinnedMeshRenderer の BlendShape を利用してキャラクタの表情を管理、運用するためのコンポーネントを提供します。

# 既知の不具合

- Redo/Undo が正しく行えない
- タイムラインクリップで LipForm の Play が出来ない

# 注意

`本機能を使用する場合、Unity の Animator や Animation で 顔の BlendShape に変更を行わないでください。`

# 前提 - BlendShape

Mesh には表情を作るための BlendShape が存在している必要があります。

用意する BlendShape は、変化する部位と性質が明確に区分されている必要があります。

以下に定義されている区分を表情を構成する BlendShape として認識します。

| 変化部位と性質 | 接頭辞 | 例 | 注釈 |
| -------- | -------------- | ---------------------------------- | ------------- |
| 瞳の模様 | IrisType_{XXX} | 瞳にハートが映っている<br> 瞳に星が映っている<br> etc. |               |
| 瞳の形状 | IrisForm_{XXX} | 瞳が小さくなる<br> etc.                   | 瞬きにも利用される     |
| 眉の屈曲 | Eyebrow_{XXX}  | 困り眉（／  ＼）<br> 怒り眉（＼  ／）<br> etc.   |               |
| 目の形状 | EyeForm_{XXX}  | 目を瞑っている<br> 片目だけ瞑っている<br> etc.     |               |
| 唇の形状 | LipForm_{XXX}  | 口を開いている<br> 口角が上がっている<br> etc.     | リップシンクにも利用される |

BlendShape の名称の接頭辞が上記と一致したものをその区分として認識します。

{XXX} の部分は作成した BlendShape の状態を示す固有名詞をアッパーキャメルケースで定めてください。

※{XXX} 部分に使用できる文字はアルファベットの大文字と小文字のみで、記号は使用しないでください。

# 設定 - Facial Expression

SkinnedMeshRenderer に `FaceController` コンポーネントを付与します。

![](Documentation~/Inspector.png)

SkinnedMesRenderer に[前提](#前提-blendshape)を満たしたメッシュが設定されている場合、Inspector には各要素が表示されます。

- [Setting](#setting)
- [表情を構成する区分毎の設定項目](#表情を構成する区分毎の設定項目)
  - [IrisType](#iristype-瞳の模様)
  - [IrisForm](#irisform-瞳の形状)
  - [Eyebrow](#eyebrow-眉の屈曲)
  - [EyeForm](#eyeform-目の形状)
  - [LipForm](#lipform-唇の形状)

## Setting

表情の状態を保存してある設定ファイルを選択するフィールドです。

![](Documentation~/Setting.png)

何も設定されていない状態でも表情の変更は可能ですが、再生開始や停止、Inspector の表示を消すと値がリセットされます。

値を保存するためには設定ファイルを生成する必要があります。

設定ファイルの生成は `Generate` のボタンを押すことで行うことができ、その時点で設定されている設定項目の値が保存されます。

## 表情を構成する区分毎の設定項目

Mesh に存在する BlendShape の中から [前提](#前提-blendshape) で定義されている接頭辞で各区分毎に列挙されます。

区分毎に編集できるフォーマットが異なります。

### IrisType - 瞳の模様

![](Documentation~/IrisType.png)

瞳の模様を変更するための BlendShape が列挙されます。

瞳の模様は表面に現れるメッシュが異なる場合が主となるため、中間の状態が使用されると見た目に問題が生じる場合があります。

この状態を避けるため、トグルによる設定のみが可能な状態となります。

### IrisForm - 瞳の形状

![](Documentation~/IrisForm.png)

瞳の形状を変更するための BlendShape が列挙されます。

瞳の形状は中間の状態が使用されても見た目に問題生じることが少ないため、スライダーによって任意の値が設定できます。

### Eyebrow - 眉の屈曲

![](Documentation~/Eyebrow.png)

眉の屈曲を変更するための BlendShape が列挙されます。

眉の屈曲は中間の状態が使用されても見た目に問題生じることが少ないため、スライダーによって任意の値が設定できます。

### EyeForm - 目の形状

![](Documentation~/EyeForm.png)

目の形状を変更するための BlendShape が列挙されます。

目の形状は瞬きにも使用されるため、目を閉じている状態と開いてい状態の２次点を設定が出来ます。

BlendShape の設定値に左辺と右辺が異なるモノが無い場合は瞬きを行わない状態となり、Inspector には警告が表示されます。

![](Documentation~/EyeFormWarning.png)

瞬きが動作する場合、Unity が再生されている状況では自動的に瞬きが行われ、非再生中においては `瞬きの変化量` の項目が表示され、スライダーによる瞬き状態をテスト出来ます。

![](Documentation~/EyeFormSimulate.png)

設定する値は左辺が瞬きによって目が閉じている状態、右辺が目を開いている状態を設定します。

また、BlendShape 名の左側にチェックボックスがありますが、こちらはチェックが入っている場合、左辺と右辺を反転した状態として扱います。

反転を使用した例としては以下があげられます
- 両目を閉じる BlendShape にチェックを入れた状態で 0～1 を設定する
- 片目を瞑る BlendShape を 0～1 に設定し、両目を瞑る BlendShape にチェックを入れた状態で 0～1 に設定します。

### LipForm - 唇の形状

![](Documentation~/LipForm.png)

唇の形状を変更するための BlendShape が列挙されます。

唇の形状はリップシンクにも使用されるため、口を閉じている状態と開いている状態の２次点を設定が出来ます。

BlendShape の設定値に左辺と右辺が異なるモノが無い場合はリップシンクを行わない状態となり、Inspector には警告が表示されます。

![](Documentation~/LipFormWarning.png)

リップシンクが動作する場合、Unity が再生されている状況では Play を押すことでリップシンクが行われます。

![](Documentation~/LipFormPlay.png)

非再生中においては `リップシンクの変化量` の項目が表示され、スライダーによるリップシンク状態をテスト出来ます。

![](Documentation~/LipFormSimulate.png)

設定する値は口の形状によって左辺と右辺の状態が相対的に変化します。

閉じた状態で唇の形状を変える BlendShape を扱う場合は、左辺を開いている状態、右辺を閉じている状態として設定します。

口を開きつつ形状をを変える BlendShape を扱う場合は、逆に左辺を閉じている状態、右辺を開いている状態として設定します。

また、BlendShape 名の左側にチェックボックスがありますが、こちらはチェックが入っている場合、左辺と右辺を反転した状態として扱います。

# タイムライン

タイムラインで表情を設定するトラック、クリップを提供しています。

`Zan.FacialExpression/Face Controller Track` を選択することでトラックが追加されます。

FaceControllerTrack にクリップを追加すると、[FaceController の Inspector](#設定---facial-expression) と同じ項目が設定できる状態となります。

FaceController で設定する場合と異なり、[Setting](#Setting) に設定ファイルを設置していない状態であっても状態は保存されます。

