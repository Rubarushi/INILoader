# INILoader
A INILoader for Windows
# Usuage
> example.ini  
```ini
[Test]
value = 0
```
> Program Code  
```cs
INILoader loader = new INILoader("example.ini");
loader.SetTitle("Test");
int value = loader.LoadInt("value", -1);
```
