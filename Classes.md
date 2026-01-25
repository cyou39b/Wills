# All Classes

下面是所有類別(Class)的列表，可以在對應的file中看到那個Class的用途。

| Classes |
| :- |
| [GlobalVariables](Assets/GlobalVariables.cs) |
| [CameraMovement](Assets/CameraMoment.cs) |
| [Explode](Assets/Explosion/Explode.cs) | 
| [FpsSliderLogic](Assets/Ingame%20Menu/FpsSliderLogic.cs) |
| [MenuManager](Assets/Ingame%20Menu/MenuManager.cs) |
| [MouseInteract](Assets/Ingame%20Menu/MouseInteract.cs) |
| [OptionMenu](Assets/Ingame%20Menu/OptionMenu.cs) |
| [VolumeSliderLogic](Assets/Ingame%20Menu/VolumeSliderLogic.cs) |
| [mainMenuLogic](Assets/MainMenu/MainMenuLogic.cs) | 
| [Bullet](Assets/WorldFight/Bullet/Bullet.cs) |
| [Wills1](Assets/WorldFight/Wills1/Wills1.cs) |
| [Gun](Assets/WorldFight/Gun.cs) |
| [HPBar](Assets/WorldFight/HPBar.cs) |
| [Jack](Assets/WorldFight/Jack.cs) |
| [JackMining](Assets/WorldMining/JackMining.cs) |
| [MachineLogic](Assets/WorldMining/MachineLogic.cs) |
| [MapSceneSwitcher](Assets/WorldMining/MapScenesSwicher.cs) |

# 一些比較複雜的class或是gamerObject的UML

### HPBar

HPBar 就是就血條

```mermaid
    classDiagram
    class MonoBehaviour {
        +Transform transform
    }
    MonoBehaviour <|-- 使用HP Bar的Class : Inheritance
    MonoBehaviour <|-- HP Bar : Inheritance


    使用HP Bar的Class --* HP Bar : Composition<br>需要目標的transform<br>來計算自己的transform
    使用HP Bar的Class *-- HP Bar : Composition<br>調用setHP和setMaxHP

    class HP Bar {
        +float HP
        +float MaxHP
        setHP()
        setMaxHP()
    }
```

### MachineLogic

負責控制ore的生成、位置、和玩家的互動計算

```mermaid
    classDiagram

    class MonoBehaviour {}
    MonoBehaviour <|-- Jack : Inheritance
    MonoBehaviour <|-- MachineLogic : Inheritance

    class Jack {}
    Jack *-- MachineLogic : Composition<br>MachineLogic <br>  是Jack 的 child gameObject


    class MachineLogic {
        +List < Vector2 > UndetectedMines
        +List < GameObject > DetectedMines
    }

    class ore的GameObject {}
    ore的GameObject --> MachineLogic : Association<br>所有被偵測到的Ore會以GameObject的形式存在，存在一個List中
```
