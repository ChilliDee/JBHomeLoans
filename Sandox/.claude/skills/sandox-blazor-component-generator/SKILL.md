---
name: sandox-blazor-component-generator
description: Generates Blazor Server components for the Sandox project in the _C / _RC / _S pattern, with no database and no logging (data comes from in-memory/mock sources). Use whenever the user asks to create a Blazor component, page, code-behind, razor file, or scaffold a new component in Sandox, or mentions the _C / _S / _RC naming pattern. Derived from the anthropic-skills:blazor-component-generator skill, adapted for Sandox.
---

# Sandox Blazor Component Generator

Generates complete Blazor component files for the **Sandox** project. Sandox uses the same `_C` / `_RC` / `_S` pattern as the CRM front end, but has **no database and no error logging**: no `IUnitOfWork`, no `IFrontEndErrorLog_B`, no `CoreException` / `FrontEndException`. Request containers return in-memory (mock) data.

The working reference is `Components/Domains/Pages/Demo/A/Demo_P_List_C.razor.cs`. When in doubt, match it.

Given a component name (e.g. `Demo_P_List_C`), always generate **two files** next to each other:
- `FileName.razor` — the razor/frontend file
- `FileName.razor.cs` — the code-behind file (contains `_C`, `_RC`, and `_S` classes)

Read the reference files before generating:
- `references/codebehind.md` — full code-behind template and rules
- `references/razor.md` — razor file template and rules
- `references/inputs.md` — the input components that exist in Sandox and how to use them
- `references/rc-patterns.md` — RC sub-class patterns and ExecuteAsync rules

---

## Before You Generate

If the user has not provided a component name, **always ask for it before doing anything else**. Do not infer or invent a name.

Also confirm where the file should live (folder under `Components/`). If the user doesn't say, default to `Components/Domains/Pages/...` for pages and `Components/Domains/...` for other components, and say which folder you chose.

> ⚠️ **BOILERPLATE REMINDER — READ THIS BEFORE EVERY GENERATION**
> Output the **complete** code-behind and razor files using the full templates from `codebehind.md` and `razor.md`. Every inner class, every lifecycle method, and the `_S` class must be present in full — even if empty. Do not abbreviate or replace any section with a comment like `// ... rest of boilerplate`. No exceptions, however simple the component.

---

## Namespace — derived from the folder path

Never hard-code a namespace. The namespace is the project root namespace `FE` plus the folder path of the file, with `.` separators. The razor file gets this namespace automatically, so the code-behind **must** match it or the partial class will not merge.

| File location | Namespace |
|---|---|
| `Components/Domains/Pages/Demo/A/X_C.razor.cs` | `FE.Components.Domains.Pages.Demo.A` |
| `Components/Domains/Utlility/Modal/X_C.razor.cs` | `FE.Components.Domains.Utlility.Modal` |

Folder names are used exactly as they are on disk — including the existing `Utlility` misspelling. Use file-scoped namespaces (`namespace FE....;`).

`_S` classes are registered automatically: `Program.cs` registers every non-abstract class ending in `_S` whose namespace starts with `FE.Components` as scoped. A component under `Components/` therefore needs **no manual DI registration** for its `_S`.

---

## Core Architecture Summary

Every component consists of exactly three classes stamped from the file name:

### `FileName_C` — The Component
- Inherits `ComponentBase, IDisposable`
- All behaviour delegated to inner classes — the component itself only wires things up
- Inner classes: `Data`, `Dependencies`, `Request`, `Inputs`, `StateSub`, `State`, `Navigation`, `Action`, `Switch`
- Lifecycle: `OnInitializedAsync` → `OnAfterRenderAsync` → `SetupAsync`

### `FileName_RC` — Request Classes
- Component-local, instantiated with `new` (never injected)
- One or more sub-classes (`Initialise_HD` is always present, others added as needed e.g. `SaveRow`, `DeleteFile`)
- Each sub-class follows an identical structure — see `references/rc-patterns.md`
- **No DB, no logging.** Data is produced in-memory inside `ExecuteAsync`, with a `Task.Delay` to simulate a request

### `FileName_S` — State Service
- Scoped service shared across components, resolved from `IServiceProvider`
- Contains `RootClass`, `RefreshClass`, and optionally additional named sub-classes
- All properties are `ObservableProperty<T>`
- Has a `Reset()` method

---

## Inner Class Responsibilities

| Class | Responsibility |
|-------|---------------|
| `Data` | Component-local state, inputs, render flags (`DidFirstRender`, `InitialisationCompleted`) |
| `Dependencies` | Shared context passed to all inner classes. Holds `I`, `RC_Internal`, `StateUpdate`, `NavManager`, `PendingOperations`, `S_Internal`, and any other resolved `_S` services |
| `Request` | Calls `ExecuteAsync` on RC sub-classes only. Thin wrappers that build a DTORequest from `_d` and call execute |
| `Inputs` | Sets initial input values on `_d.I.Inputs`; holds `AddValidator` / `ValidateAll` |
| `StateSub` | Subscribes/unsubscribes to `ObservableProperty` values on this or other components' `_S`. One exception: may call RC directly in subscription callbacks. **Every subscribe call must pass `_d.I.ClassName` as the final `subscribedFrom` argument.** |
| `State` | Writes to `ObservableProperty` values on this or other components' `_S` |
| `Navigation` | Handles route navigation via `_d.NavManager` |
| `Action` | General-purpose logic, calculations, boolean visibility helpers, caution modals. Always includes `CompletePendingOperationsAsync()` |
| `Switch` | Returns CSS class strings only. Never returns booleans or does logic |

---

## Critical Rules

1. **No method calling methods** — methods in inner classes must be flat and self-contained. No method may call another method within the same class (except `ExecuteAsync`, the orchestrator).
2. **`ExecuteAsync` is the only orchestrator** — it calls methods in vertical sequence; each method stores results into `_i`, never returns values.
3. **`_req` calls RC only** — `_req` methods only build a DTORequest and call `ExecuteAsync`. The one exception is `StateSub`, which may call RC directly in subscription callbacks.
4. **⚠️ ALL BOILERPLATE MUST ALWAYS BE PRESENT — NO EXCEPTIONS** — Every inner class (`Data`, `Dependencies`, `Request`, `Inputs`, `StateSub`, `State`, `Navigation`, `Action`, `Switch`) and the `_S` class (`{FileName}_S`, `RootClass`, `RefreshClass`) must appear in full in every generated component, exactly as in `codebehind.md`. Even with no custom logic, include the empty boilerplate form. Never omit, collapse, or summarise any class — even if the user says "just a simple component".
5. **`_S` Reset()** — when a component owns its `_S`, call `_d.S_Internal.Reset()` in `Dispose()`.
6. **Loading gate** — always combine `InitialisationCompleted` and `RequestIsLoading` in the same `@if` block.
7. **Injecting an external `_S` class** — only add the `_S` field to `Dependencies` and resolve it there (e.g. `public SomeOther_S S_Other { get; set; }`). Do **not** add methods, subscriptions, or property bindings to `StateSub` or `State` unless the user explicitly asks.
8. **`subscribedFrom` is required on all subscribe calls** — every `Subscribe*` method on `ObservableProperty` (`SubscribeAsync`, `NoDataSubscribeAsync`, `Subscribe`, `SubscribeConditional`, etc.) requires `_d.I.ClassName` as the final argument. Unsubscribe methods do **not** take it.
9. **No database, no logging** — never add `IUnitOfWork`, `IFrontEndErrorLog_B`, repositories, `CoreException`, `FrontEndException`, or any logging `using`s. If the user asks for data, generate an in-memory/mock source inside the RC sub-class and say so.

---

## Razor onclick Pattern

```razor
@onclick="async() => {
    await _req.SomeRequestAsync();
    _state.SomeStateMethod();
    await _d.StateUpdate();
}"
```

- `_req` — call RC execute methods
- `_state` — write to observable properties
- `_act` — general logic / calculations
- `_switch` — CSS strings only, used in `class="@_switch.GetSomeCss()"`
- `_act` boolean methods — used in `@if (_act.ShowSomething())`

---

## Input Selection Rules

- **Inside a table** → use `FE.Components.Domains.Utlility.Inputs.Table.*`
- **Outside a table** → use `FE.Components.Domains.Utlility.Inputs.*`
- Never mix table inputs outside tables or standard inputs inside tables

Only use inputs listed in `references/inputs.md` — they are the components that actually exist in Sandox, under their real file names (e.g. `DatePickerInput`, `MinMaxCurrencyInput_C`). Option types and enums for inputs are nested inside their component; see the "Nested option types" section in `references/inputs.md`.

---

## Input Population Rules

For every input, explicitly declare in the `Inputs` class (as a comment above each property) whether it is **pre-populated from mock/initial data** or **left blank/unselected on load**.

- **Pre-populated** — the value comes from the RC's mock/initial data during `Initialise_HD` and is assigned in the `Inputs` class. Comment: `// Pre-populated from mock/initial data`
- **Blank** — the input starts empty or unselected. Comment: `// Left blank on load`

**Dropdowns** follow a split rule:
- The **options list** is always loaded (always produced in `Initialise_HD`)
- The **selected value** follows the same pre-populated vs blank rule — declare it explicitly with the same comments

**If the user has not specified** whether an input is pre-populated or blank, **stop and ask before generating**. Do not assume.

**Validation** — for every input, also ask whether it is **required** or **optional**:
- **Required** — add a validator via `_inputs.AddValidator(...)` in `SetInitialInputs()`
- **Optional** — no validator added; omit from validation entirely

**If the user has not specified** required vs optional, **stop and ask before generating**. Do not assume.

---

## Pre-Generation Plan Summary

Once all requirements are understood and all questions answered, **before writing any code**, output a plan summary in this format. The user must be able to review and confirm it before generation begins.

```
### Plan Summary

**Location**
Folder, file names, and resulting namespace.

**Inner Class Methods**
List every non-boilerplate method you intend to add to each inner class of `_C`.
Format: ClassName → method signatures, one per line.
Omit classes that will only contain standard boilerplate.

Example:
- Request     → InitialRequestAsync(), SaveRowAsync()
- Inputs      → SetInitialInputs() [StartDate (pre-pop), Amount (blank)]
- StateSub    → Subscribe/UnSubscribe for: SomeOther_S.Root.SelectedId
- State       → SetSelectedId()
- Action      → ShowConfirmPanel() : bool
- Switch      → GetRowCss() : string

**RC Sub-Classes**
List every sub-class you intend to add inside `_RC` (in addition to the always-present `Initialise_HD`), and where its mock data comes from.
Format: ClassName — one-line description.

Example:
- Initialise_HD  — returns three mock rows and the dropdown options (in-memory)
- SaveRow_HD     — simulates saving the edited row (Task.Delay, no persistence)

**_S State**
List every `ObservableProperty` you intend to add to `_S` sub-classes (Root or custom).
Format: SubClass.PropertyName : Type — one-line description.

Example:
- Root.SelectedId : int?  — the currently selected row ID
- Root.Rows : List<RowDto>  — the loaded grid rows
```

After presenting the plan, ask: **"Does this look right? Happy for me to go ahead?"**

Do not begin generating files until the user confirms.

---

## After Generating

- Tailwind is prebuilt (`wwwroot/output.css`). If the razor uses classes that aren't already compiled, remind the user to run `npm run build:css` (or `npm run watch:css`) from the `Sandox` project folder.
- Say plainly that the generated code has not been compiled unless you actually built it.
