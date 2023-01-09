# VMC Protocol Extension for VMCTransportBridge

[VMC Protocol Specification](https://protocol.vmc.info/english)

### Available（利用可否）
```
/VMC/Ext/OK/Transported (int){loaded} (int){calibration state} (int){calibration mode} (int){tracking status} (int){NetworkClientId}
```

### Local VRM information（ローカルVRM基本情報）
```
/VMC/Ext/VRM/Transported (string){path} (string){title} (string){Hash} (int){NetworkClientId}
```

### Remote VRM information（リモートVRM基本情報）
```
/VMC/Ext/Remote/Transported (string){service} (string){json} (int){NetworkClientId}
```

### Relative Time（送信側相対時刻）
```
/VMC/Ext/T/Transported (float){time} (int){NetworkClientId}
```

### Root Transform（Root姿勢）
```
/VMC/Ext/Root/Pos/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (float){s.x} (float){s.y} (float){s.z} (float){o.x} (float){o.y} (float){o.z} (int){NetworkClientId}
```

### Bone Transform（Bone姿勢）
```
/VMC/Ext/Bone/Pos/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkClientId}
```

### VRM BlendShapeProxyValue
```
/VMC/Ext/Blend/Val/Transported (string){name} (float){value} (int){NetworkClientId}
/VMC/Ext/Blend/Apply/Transported (int){NetworkClientId}
```

### Camera Transform & FOV（Camera位置・FOV）
```
/VMC/Ext/Cam/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (float){fov} (int){NetworkClientId}
```

### Directional Light Transform & Color（DirectionalLight位置・色）
```
/VMC/Ext/Light/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (float){color.red} (float){color.green} (float){color.blue} (float){color.alpha} (int){NetworkClientId}
```

### Controller Input
```
/VMC/Ext/Con/Transported (int){active} (string){name} (int){IsLeft} (int){IsTouch} (int){IsAxis} (float){Axis.x} (float){Axis.y} (float){Axis.z} (int){NetworkClientId}
```

### Keyboard Input
```
/VMC/Ext/Key/Transported (int){active} (string){name} (int){keycode} (int){NetworkClientId}
```

### MIDI Note Input
```
```

### MIDI CC Value Input
```
```

### MIDI CC Button Input
```
```

### Device Transform
```
/VMC/Ext/Hmd/Pos/Transported (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkCliendId}

/VMC/Ext/Con/Pos/Transported (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkCliendId}

/VMC/Ext/Tra/Pos/Transported (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkCliendId}


/VMC/Ext/Hmd/Pos/Local/Transported (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkCliendId}

/VMC/Ext/Con/Pos/Local/Transported (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkCliendId}

/VMC/Ext/Tra/Pos/Local/Transported (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (int){NetworkCliendId}
```
