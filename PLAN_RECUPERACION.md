# 🔧 Plan de Recuperación del Proyecto CHURROOOS

## ⚠️ SITUACIÓN ACTUAL

**PROBLEMA CRÍTICO DETECTADO:** Múltiples archivos de código fuente (.cs) están **CORRUPTOS**.

### Archivos Afectados
- ❌ `GameManager.cs`
- ❌ `PlayerMovement.cs`
- ❌ `PlayerStats.cs`
- ❌ `UIManager.cs`
- ❌ `DialogoManager.cs`
- ❌ `PlayerActions.cs`
- ❌ `PlayerInteraction.cs`

### Síntomas
- Código mezclado y desordenado
- Líneas de código superpuestas
- Caracteres especiales corruptos (�)
- Estructura de clases incompleta o ilegible

---

## 🔍 DIAGNÓSTICO

### Causa Probable
La corrupción parece ser resultado de:
1. **Problemas de merge en Git** - Conflictos no resueltos correctamente
2. **Codificación de caracteres** - Mezcla de UTF-8, UTF-8 BOM, y otros encodings
3. **Editor de código** - Posible corrupción durante el guardado

### Verificación
- ✅ El historial de Git existe (19 commits)
- ⚠️ Los archivos están corruptos INCLUSO en commits anteriores
- ⚠️ Esto sugiere que la corrupción ocurrió durante múltiples commits

---

## 🚨 OPCIONES DE RECUPERACIÓN

### Opción 1: Reconstrucción Manual (RECOMENDADA)
**Tiempo estimado:** 2-4 horas  
**Dificultad:** Media  
**Éxito:** 95%

#### Pasos:
1. **Analizar fragmentos legibles** de cada archivo corrupto
2. **Reconstruir la lógica** basándose en:
   - Fragmentos de código visible
   - Nombres de variables y métodos
   - Estructura de clases detectada
   - Contexto del proyecto
3. **Crear versiones limpias** de cada archivo
4. **Probar compilación** incremental

#### Ventajas:
- Control total sobre el código
- Oportunidad de mejorar la arquitectura
- Documentar mientras reconstruyes

#### Desventajas:
- Requiere tiempo
- Posible pérdida de lógica específica

---

### Opción 2: Búsqueda de Backups
**Tiempo estimado:** 30 minutos - 2 horas  
**Dificultad:** Baja  
**Éxito:** Variable (depende de si existen backups)

#### Lugares donde buscar:
1. **Carpeta de Unity:**
   - `Library/StateCache/`
   - Archivos temporales de Unity

2. **Backups del sistema:**
   - Historial de archivos de Windows
   - OneDrive/Google Drive (si está sincronizado)
   - Copias de seguridad automáticas

3. **Otros repositorios:**
   - Otros clones del repositorio Git
   - Versiones en otras computadoras

4. **Papelera de reciclaje:**
   - Versiones anteriores eliminadas

---

### Opción 3: Reescritura Completa
**Tiempo estimado:** 6-10 horas  
**Dificultad:** Alta  
**Éxito:** 100% (pero desde cero)

#### Cuándo considerar:
- Si las opciones 1 y 2 fallan
- Si quieres mejorar significativamente la arquitectura
- Si tienes tiempo antes del Jam

---

## 🛠️ PLAN DE ACCIÓN RECOMENDADO

### Fase 1: Evaluación (15 minutos)
- [ ] Buscar backups locales
- [ ] Verificar historial de archivos de Windows
- [ ] Revisar otros clones del repositorio

### Fase 2: Reconstrucción (2-4 horas)
Si no hay backups, proceder con reconstrucción manual:

#### 1. GameManager.cs
**Fragmentos detectados:**
```csharp
public static GameManager Instance { get; private set; }
[Header("Estado del Juego")]
public int diaActual = 1;
void IrAlMenu() { SceneManager.LoadScene("MenuScene"); }
void CargarEscenaJuego() { ... }
```

**Funcionalidad inferida:**
- Singleton pattern
- Gestión de escenas
- Control de días
- Pausa del tiempo

#### 2. PlayerMovement.cs
**Fragmentos detectados:**
```csharp
[RequireComponent(typeof(Rigidbody2D))]
public float walkSpeed = 5f;
public float runSpeed = 8.5f;
private float currentSpeed;
// Sistema de flip
transform.localScale = new Vector...
```

**Funcionalidad inferida:**
- Movimiento con Rigidbody2D
- Velocidades de caminar/correr
- Flip del sprite según dirección

#### 3. PlayerStats.cs
**Fragmentos detectados:**
```csharp
public float money = 0f;
public int churrosCantidad = 10;
public float hydration = 100f;
public float hydrationMax = 100f;
public float stamina = 100f;
public float staminaMax = 100f;
```

**Funcionalidad inferida:**
- Stats del jugador
- Sistema de límites (max values)
- Gestión de recursos

#### 4. UIManager.cs
**Fragmentos detectados:**
```csharp
public Slider heatSlider;
public TextMeshProUGUI churrosText;
churrosText.text = "CHURROS: " + "10";
```

**Funcionalidad inferida:**
- Referencias a UI
- Actualización de sliders
- Actualización de textos

#### 5. DialogoManager.cs
**Fragmentos detectados:**
```csharp
public static DialogoManager Instance { get; private set; }
stats.money -= npcActual.precioAgua;
npcActual.FinalizarVenta();
```

**Funcionalidad inferida:**
- Singleton pattern
- Sistema de compra/venta
- Gestión de NPCs

#### 6. PlayerActions.cs
**Fragmentos detectados:**
```csharp
public void TomarAgua(float cantidad)
stats.hydration = ...
stats.temperature = Mathf.Max(stats.temperature - ...)
public void ComerChurro()
```

**Funcionalidad inferida:**
- Acción de tomar agua
- Recuperación de hidratación
- Reducción de temperatura
- Consumo de churros

#### 7. PlayerInteraction.cs
**Fragmentos detectados:**
```csharp
public float radioInteraccion = 2.5f;
public LayerMask capaNPC;
// Detección de NPCs
npc.Interactuar();
```

**Funcionalidad inferida:**
- Sistema de interacción por radio
- Detección de NPCs con LayerMask
- Trigger de interacciones

---

### Fase 3: Compilación y Testing (1 hora)
- [ ] Compilar proyecto
- [ ] Resolver errores de compilación
- [ ] Probar funcionalidad básica
- [ ] Verificar que todas las referencias están conectadas

### Fase 4: Commit y Backup (15 minutos)
- [ ] Hacer commit de archivos reparados
- [ ] Crear backup completo del proyecto
- [ ] Documentar cambios realizados

---

## 📋 CHECKLIST DE RECUPERACIÓN

### Pre-Recuperación
- [ ] Hacer backup completo del proyecto actual
- [ ] Crear nueva rama en Git: `git checkout -b recuperacion-archivos`
- [ ] Documentar estado actual

### Durante Recuperación
- [ ] Reconstruir GameManager.cs
- [ ] Reconstruir PlayerMovement.cs
- [ ] Reconstruir PlayerStats.cs
- [ ] Reconstruir UIManager.cs
- [ ] Reconstruir DialogoManager.cs
- [ ] Reconstruir PlayerActions.cs
- [ ] Reconstruir PlayerInteraction.cs
- [ ] Verificar PlayerHealthSystem.cs (parece menos afectado)

### Post-Recuperación
- [ ] Compilación exitosa
- [ ] Testing de movimiento
- [ ] Testing de interacciones
- [ ] Testing de UI
- [ ] Testing de diálogos
- [ ] Merge a main: `git checkout main && git merge recuperacion-archivos`

---

## 🔧 HERRAMIENTAS ÚTILES

### Comandos Git
```bash
# Ver historial de un archivo específico
git log --all --full-history -- "ruta/al/archivo.cs"

# Ver contenido de un archivo en un commit específico
git show <commit-hash>:ruta/al/archivo.cs

# Crear rama de recuperación
git checkout -b recuperacion-archivos

# Comparar con versión anterior
git diff HEAD~5 HEAD -- Assets/Scripts/
```

### Verificación de Encoding
```powershell
# Ver encoding de un archivo
[System.IO.File]::ReadAllText('ruta\al\archivo.cs', [System.Text.Encoding]::UTF8)

# Convertir a UTF-8
$content = Get-Content 'archivo.cs' -Raw
[System.IO.File]::WriteAllText('archivo.cs', $content, [System.Text.Encoding]::UTF8)
```

---

## 💡 PREVENCIÓN FUTURA

### 1. Mejorar .gitignore
```gitignore
# Visual Studio
.vs/
*.vsidx
*.v2
*.suo
*.user

# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Ll]ogs/
[Uu]ser[Ss]ettings/
```

### 2. Configurar Git
```bash
# Asegurar encoding UTF-8
git config --global core.autocrlf true
git config --global core.safecrlf warn

# Configurar merge tool
git config --global merge.tool vscode
```

### 3. Backups Automáticos
- Configurar backup automático en Unity
- Usar control de versiones en la nube (GitHub/GitLab)
- Backups locales diarios

### 4. Buenas Prácticas
- Commits frecuentes con mensajes descriptivos
- Nunca hacer force push sin backup
- Revisar diffs antes de commitear
- Usar branches para features nuevas

---

## 📞 SIGUIENTE PASO

**¿Quieres que proceda con la reconstrucción de los archivos?**

Puedo:
1. ✅ **Reconstruir todos los archivos corruptos** basándome en los fragmentos detectados
2. ✅ **Crear versiones limpias y funcionales** de cada script
3. ✅ **Añadir comentarios y documentación** mientras reconstruyo
4. ✅ **Mejorar la arquitectura** donde sea necesario

Solo necesito tu confirmación para comenzar. El proceso tomará aproximadamente 2-3 horas, pero tendrás un proyecto funcional y bien documentado.

---

**Fecha:** 2026-01-08  
**Autor:** Antigravity AI  
**Versión:** 1.0
