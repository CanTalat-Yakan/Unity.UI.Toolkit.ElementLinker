# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# UI Toolkit Element Linker

> Quick overview: Bind MonoBehaviours to UI Toolkit VisualElements without boilerplate. Create a link to a single element or a query of elements by type, then access them at runtime via strongly‑typed helpers.

This module wires scene scripts to UI Toolkit elements in a UIDocument. You can create a link to a specific element selected in UI Builder, or add a query that collects all elements of a given type. Linked references are refreshed automatically and shown in a friendly inspector.

![screenshot](Documentation/Screenshot.png)

## Features
- Link to a specific element
  - `UIElementLink` stores a path to a VisualElement inside a `UIDocument` and resolves it at runtime
  - One‑click “Create Link for …” button appears inside UI Builder’s inspector
- Query multiple elements
  - `UIElementQuery` gathers all VisualElements of a chosen `UIElementType` under the document root
- Script helper base classes
  - `UIScriptComponentBase` and `UIScriptComponentBase<T>` expose `LinkedElements` and convenience iterators
- Live refresh and feedback
  - Links refresh in `Awake`/`OnEnable`; inspector shows the resolved type/name and the source UI asset
  - Selecting a link in the Hierarchy syncs selection in UI Builder to the linked element path
- Editor menu integration
  - GameObject → UI Toolkit → Add Query creates a pre‑configured `UIElementQuery` under a selected `UIDocument`

## Requirements
- Unity 6000.0+
- UI Toolkit (`UIDocument`, UXML/USS)
- Optional (for nicer inspector UI): UnityEssentials attribute modules providing `[Info]`, `[Button]`, `[OnValueChanged]`

## Usage

### Link a specific element from UI Builder
1) Open your UXML in UI Builder and select the target element
2) Use the “Create Link for …” button (added to the UI Builder inspector by this package)
3) A new GameObject is created under the matching `UIDocument` and receives a `UIElementLink`
4) In your script, read `link.LinkedElement` or subscribe to `link.OnRefreshLink`

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using UnityEssentials;

public class HealthBarController : MonoBehaviour
{
    public UIElementLink HealthBarLink; // assigned in Hierarchy after creating link

    void OnEnable()
    {
        HealthBarLink.OnRefreshLink += Setup;
        Setup(HealthBarLink.LinkedElement); // also handle already-resolved
    }

    void Setup(VisualElement el)
    {
        if (el == null) return;
        // e.g., query sub-elements, register callbacks, set initial state
        var bar = el.Q<VisualElement>("fill");
        bar.style.width = 0f;
    }
}
```

### Query by element type
1) Select a `UIDocument` GameObject in the Hierarchy
2) Menu: GameObject → UI Toolkit → Add Query
3) Choose the element type (e.g., Button) on the new `UIElementQuery`
4) Read `query.LinkedElements` or subscribe to `query.OnRefreshLinks`

```csharp
public class ButtonBinder : MonoBehaviour
{
    public UIElementQuery Query; // set to Button type in inspector

    void OnEnable()
    {
        foreach (var el in Query.LinkedElements)
            (el as Button)?.clicked += () => Debug.Log($"Clicked {el.name}");
    }
}
```

### Use the script helper base
```csharp
using UnityEssentials;
using UnityEngine.UIElements;

public class ToggleHighlighter : UIScriptComponentBase<Toggle>
{
    void OnEnable()
    {
        IterateLinkedElements(t => t.RegisterValueChangedCallback(_ => UpdateStyle(t)));
        IterateLinkedElements(UpdateStyle);
    }

    void UpdateStyle(Toggle t)
    {
        if (t.value) t.AddToClassList("on");
        else t.RemoveFromClassList("on");
    }
}
```

## How It Works
- `UIElementLink`
  - Stores a serialized path (`UIElementPathEntry[]`) to the target element within the `UIDocument.rootVisualElement`
  - Resolves the path via utilities and exposes it as `LinkedElement`; refreshes on `Awake`/`OnEnable` or when you call `RefreshLink()`
  - Editor hook shows a button in UI Builder to create a link GameObject under the correct `UIDocument`
- `UIElementQuery`
  - Traverses the document tree and collects elements whose runtime type maps to the chosen `UIElementType`
  - Exposes the array via `LinkedElements`; refreshes on `Awake`/`OnEnable` and when the type changes
- `UIScriptComponentBase`
  - Detects an adjacent `UIElementLink` or `UIElementQuery`, caches the `UIDocument` and linked elements, and offers iteration helpers
- Editor synchronization
  - Selecting a link GameObject in the Hierarchy selects the matching element path in UI Builder for easy editing

## Notes and Limitations
- A `UIDocument` parent is required; links/queries resolve against its `rootVisualElement`
- Element paths rely on hierarchy and names; renaming or restructuring UXML may break existing links (re‑link as needed)
- `UIElementType` covers common controls; some custom elements may not map automatically
- Links/queries resolve in Edit and Play Mode (`[ExecuteAlways]`); guard your runtime logic accordingly

## Files in This Package
- Runtime
  - `Runtime/UIElementLink.cs` – Single‑element linking via serialized path
  - `Runtime/UIElementQuery.cs` – Type‑based querying over the document tree
  - `Runtime/UIScriptComponentBase.cs` – Helper base classes for consuming links/queries
  - `Runtime/UnityEssentials.UIToolkitElementLinker.asmdef`
- Editor
  - `Editor/UIElementLinker.cs` – UI Builder integration (“Create Link for …” button)
  - `Editor/UIElementLinkEditor.cs` – Hierarchy ↔ UI Builder selection sync; “Add Query” menu item
  - `Editor/UnityEssentials.UIElementLinker.Editor.asmdef`

## Tags
unity, ui toolkit, uitoolkit, uidocument, visualelement, query, binding, linker, uxml, editor-tools
