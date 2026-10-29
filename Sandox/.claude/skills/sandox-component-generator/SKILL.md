---
name: sandox-component-generator
description: Generates Blazor component files (.razor + .razor.cs with the _C, _RC and _S classes) for the Sandox project, following the DMP component pattern but with no database, no BusinessLogic package and no page lock. Use for any new Sandox component, code-behind, razor, RC sub-class or _S state work.
---

# Sandox Component Generator

Generates complete Blazor component files for the **Sandox** project (`C:\Projects\Sandox`): a standalone Blazor Server environment for building CRM components without the CRM backend.

It is based on the `blazor-component-generator` skill (the DMP platform version) and keeps the same architecture. What differs is the environment:

| DMP platform | Sandox |
|---|---|
| `BusinessLogic.Core` NuGet package | Not referenced. The pattern's base classes live in `Framework/` (same `Mortgage.State` / `Mortgage.Interceptors...` namespaces) |
| `_RC` requests read from `IUnitOfWork` | `_RC` requests build **mock data in memory** |
| Errors go to `IFrontEndErrorLog_B` (DB + Teams) | Errors go to the console log (`ILogger`) through the interceptor; `_RC` calls `SetRequestFailed(ex.Message)` |
| `CoreException` / `FrontEndException` | Do not exist. Catch `Exception` only |
| Inputs disabled by `PageLock_S`, `IgnorePageLock` parameter | Page lock removed. Never generate `IgnorePageLock`. An input is disabled only by `Disabled` |
| `AddressSearchInput` (Google Places) | Deleted. Do not generate it |
| Auth, SignalR, DB contexts | None |

Given a component name (e.g. `Ticket_P_C` for a page, `Ticket_Form_C` for its sub component), always generate **two files**:
- `FileName.razor` — the razor/frontend file
- `FileName.razor.cs` — the code-behind file (contains the `_C`, `_RC`, and `_S` classes)

Read the reference files in this skill's folder for the full templates before generating:
- `codebehind.md` — full code-behind template and rules
- `razor.md` — razor file template and rules
- `inputs.md` — all available input components, their parameters and validation support
- `rc-patterns.md` — RC sub-class patterns, mock data rules and ExecuteAsync rules

> **If a reference file conflicts with this SKILL.md, this SKILL.md wins.**

---

## Before You Generate

If the user has not provided a component name, **always ask for it before doing anything else**. Do not infer or invent a name.

> ⚠️ **BOILERPLATE REMINDER — READ THIS BEFORE EVERY GENERATION**
> Output the **complete** code-behind and razor files using the full templates from `codebehind.md` and `razor.md`. Every inner class, every lifecycle method, and the `_S` class must be present in full, even if empty. Do not abbreviate or replace any section with a comment like `// ... rest of boilerplate`. No exceptions, however simple the component.

---

## Where files go and how to name them

Folder and file naming follows the page tree. Never break these rules.

```
Components/Domains/Pages/
  <Area>/                         area folder, e.g. Test
    A/                            the page's own A folder
      <Area>_P_C.razor(.cs)       the PAGE component: area + _P, no page name. Owns @page "/route"
    <Child>/                      a sub component is a SIBLING of A, named for the child only
      A/
        <Area>_<Child>_C.razor(.cs)
      <Grandchild>/               going deeper: a sibling of the child's A, again with its own A
        A/
          <Area>_<Child>_<Grandchild>_C.razor(.cs)
```

Every component lives in an `A` folder. Sub components are folders beside `A` (siblings), and each sibling has its own `A`. To go a layer deeper, put a new folder beside that `A`, with its own `A` inside it, and so on.

Worked example (`Test` area):

| Folder | Component |
|---|---|
| `Pages/Test/A/` | `Test_P_C` (page, `@page "/test"`) |
| `Pages/Test/Hero/A/` | `Test_Hero_C` |
| `Pages/Test/ProductGrid/A/` | `Test_ProductGrid_C` |
| `Pages/Test/ProductGrid/Card/A/` | `Test_ProductGrid_Card_C` |

Rules:
- **Every component sits in an `A` folder.** Sub component folders are siblings of the parent's `A`.
- **Page** = `<Area>_P_C`. It has `_P`, carries no page name, and is the only component with `@page`. One page per area folder; a second page needs its own area folder.
- **Sub components have no `_P`.** Their file name carries the full tree from the area down (`Test_ProductGrid_Card_C`), while their folder is only the component's own name (`Card/`), because the parent folder already says what it belongs to.
- Each component's `_RC` and `_S` take the same stem: `Test_ProductGrid_Card_RC`, `Test_ProductGrid_Card_S`.
- The folder path becomes the namespace, prefixed `FE.` (`Pages/Test/ProductGrid/Card/A/` → `namespace FE.Components.Domains.Pages.Test.ProductGrid.Card.A;`).
- The page hosts its children by full tag with a `@using` for the child namespace (`@using FE.Components.Domains.Pages.Test.Hero.A` then `<Test_Hero_C />`).
- Components that repeat (a card per row) get a unique root id, e.g. `id="@($"{_d.I.ClassName}-{Item.Id}")"`, and `@key` on the tag. They take `[Parameter]`s from the parent; the parent passes callbacks (`EventCallback<T>`) rather than the child writing the parent's state.
- Shared state between a page and its children: the page's `_S` holds it. A child injects the page's `_S` in its `Dependencies` (wiring), and subscribes only when it needs to re-render on change.
- The page component itself is routable (`@page` in its own `.razor`). There is no separate thin page file under `Components/Pages/`.
- Every `*_S` class in a `FE.Components*` namespace is registered as scoped automatically by `Program.cs`. **Never add a manual `AddScoped` for an `_S` class.**
- Existing examples of the pattern: `Pages/Demo/A/Demo_P_C` (single page) and the `Pages/Test` tree (page with nested sub components).

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
- Contains one or more sub-classes (`Initialise_HD` is always present, others added as needed e.g. `SaveRow`, `DeleteFile`)
- Each sub-class follows identical structure and **returns mock data** — see `rc-patterns.md`

### `FileName_S` — State Service
- Injectable scoped service shared across components
- Contains `RootClass`, `RefreshClass`, and optionally additional named sub-classes
- All properties are `ObservableProperty<T>`
- Has a `Reset()` method

---

## Static components (no API calls): the default for now

Unless the user asks for a component that loads data, shows loading or failure, or otherwise behaves like a request, generate it as a **static component**. It still has the full `_C`, `_RC` and `_S` boilerplate, with `Initialise_HD` in the `_RC` ready for later, but:

1. **Do not call the request on startup.** `SetupAsync()` does not call `_req.InitialRequestAsync()`. Leave the `InitialRequestAsync()` method in `Request` and the `Initialise_HD` in the `_RC` untouched, so they are ready when the component starts calling something later. Do not add a call to it.
2. **No gate in the razor.** Do not use the three-state gate, and do not check `InitialisationCompleted`, `RequestIsLoading`, `RequestHasFailed` or `RequestHasCompleted`. The root element is just `<div id="@_d.I.ClassName"> ...the UI... </div>` (a wrapper with `style="display: contents"` when the markup must not gain a layout box).
3. **Inputs get their starting values directly.** With no request there is nothing to read from `ResponseData`, so `SetInitialInputs()` assigns literal defaults (or nothing, for blank inputs). Still keep the pre-populated / blank comment on each input.
4. `SetupAsync()` order for a static component: `_stateSub.Subscribe();`, then `_inputs.SetInitialInputs();`, then `await _d.StateUpdate();`.

Switch a component back to the normal gated pattern (call `_req.InitialRequestAsync()` before `_inputs.SetInitialInputs()`, use the three-state gate) as soon as the user asks for it to load data, show a loading or failed state, or demonstrate an `_RC` request. Components whose whole purpose is showing `_RC` behaviour (success, failure, retry) always use the gate.

Say in the Plan Summary whether each component is static or gated.

---

## Inner Class Responsibilities

| Class | Responsibility |
|-------|---------------|
| `Data` | Component-local state, inputs, render flags (`DidFirstRender`, `InitialisationCompleted`) |
| `Dependencies` | Shared context passed to all inner classes. Holds `_i`, `RC_Internal`, `StateUpdate`, `NavManager`, `PendingOperations`, `S_Internal`, and any injected `_S` services |
| `Request` | Calls `ExecuteAsync` on RC sub-classes only. Thin wrappers that build DTORequest from `_d` and call execute |
| `Inputs` | Sets initial input values on `_d.I.Inputs`. Always exposes `AddValidator`, `RemoveValidator` and `ValidateAll` |
| `StateSub` | Subscribes/unsubscribes to `ObservableProperty` values on this or other components' `_S`. One exception: may call RC directly in subscription callbacks. **Every subscribe call must pass `_d.I.ClassName` as the final `subscribedFrom` argument.** |
| `State` | Writes to `ObservableProperty` values on this or other components' `_S` |
| `Navigation` | Handles route navigation via `_d.NavManager` |
| `Action` | General-purpose logic, calculations, boolean visibility helpers, caution modals. Always includes `CompletePendingOperationsAsync()` |
| `Switch` | Returns CSS class strings only. Never returns booleans or does logic |

---

## Critical Rules

1. **No method calling methods** — methods in inner classes must be flat and self-contained. No method may call another method within the same class (except in `ExecuteAsync`, which is the orchestrator).
2. **`ExecuteAsync` is the only orchestrator** — it calls methods in vertical sequence; each method stores results into `_i`, never returns values.
3. **`_req` calls RC only** — `_req` methods do nothing except build a DTORequest and call `ExecuteAsync`. The one exception is `StateSub`, which may call RC directly in subscription callbacks.
4. **⚠️ ALL BOILERPLATE MUST ALWAYS BE PRESENT — NO EXCEPTIONS** — Every inner class (`Data`, `Dependencies`, `Request`, `Inputs`, `StateSub`, `State`, `Navigation`, `Action`, `Switch`) and the `_S` class (`{FileName}_S`, `RootClass`, `RefreshClass`) must appear in full, exactly as in `codebehind.md`, even when empty. Never omit, collapse or summarise any class.
5. **`_S` Reset()** — when a component's own `_S` is disposed, call `_d.S_Internal.Reset()` in `Dispose()`.
6. **Loading gate** — only for components that load or submit data (see *Static components* below). When a gate is used, always combine `InitialisationCompleted` and `RequestIsLoading` in the same `@if` block.
7. **Injecting an external `_S` class** — if the user asks for an external `_S`, **only** add the field to `Dependencies` and resolve it there (e.g. `public SomeOther_S S_Other { get; set; }`). Do **not** add methods, subscriptions or property bindings to `StateSub` or `State` unless the user explicitly asks.
8. **`subscribedFrom` is required on all subscribe calls** — every `Subscribe*` method on `ObservableProperty` requires `_d.I.ClassName` as the final argument. Unsubscribe methods do **not** take it.
9. **`Data.InputsClass` inherits `ValidatableInputs`, and the `Inputs` inner class always has these three pass-throughs**, even when unused:
   ```csharp
   public void AddValidator(Func<(bool IsValid, string Label)> validator)
       => _d.I.Inputs.AddValidator(validator);

   public void RemoveValidator(Func<(bool IsValid, string Label)> validator)
       => _d.I.Inputs.RemoveValidator(validator);

   public void ValidateAll()
       => _d.I.Inputs.ValidateAll();
   ```
10. **No backend types, ever.** Never generate `IUnitOfWork`, repositories, `IFrontEndErrorLog_B`, `CoreException`, `FrontEndException`, `BusinessLogic.*` usings, `PageLock_S`, `IgnorePageLock`, `AddressSearchInput`, `HttpClient` calls to real services, or anything that needs a connection string or API key. If the user asks for something that needs one, build a mock and say so.
11. **Mock data lives in the RC only** — see `rc-patterns.md`. The component (`_C`), razor and `_S` never fabricate data; they read `ResponseData`.
12. **Don't invent facts in mock data.** Use clearly fake, obviously placeholder values (`Mock client A`, `$120.00`) and keep them small (3–8 rows). If the user supplied real field names or sample values, use those.

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

## Inputs and Validation

### Input selection
- **Inside a table** → `FE.Components.Domains.Utlility.Inputs.Table.*`
- **Outside a table** → `FE.Components.Domains.Utlility.Inputs.*`
- Never mix table inputs outside tables or standard inputs inside tables.
- Use tag names exactly as the files are named. Current names:
  - Standard: `TextInput`, `TextArea`, `NumberInput`, `NumberDropDownInput`, `CurrencyInput`, `PercentageInput`, `MinMaxCurrencyInput_C`, `MobileInput`, `CheckBoxInput`, `TrueFalseInput`, `YesNoRadioButtonInput`, `SelectableListInput`, `MultiSelectableListInput`, `SingleSelectDropDown`, `SingleSelectDropDownWithSearch`, `SingleSelectDropDownTwoColumnDisplayWithSearch`, `MultiSelectDropdown`, `DatePickerInput` (date only), `DateTimePicker` (date + optional time), `TimePickerInput`
  - Table: `Table.TableTextInput`, `Table.TableCurrencyInput`, `Table.TablePercentageInput`, `Table.TableCheckBoxInput`, `Table.TableCheckBoxInput2`, `Table.TableSingleSelectDropDown`, `Table.TableSingleDropDownWithSearch`
  - There is no `DatePicker`, `MinMaxCurrencyInput`, `TableSingleSelectDropDownWithSearch` or `AddressSearchInput` — do not generate those names.

See `inputs.md` for each input's parameters and option types.

### Population rules
For every input, explicitly declare in the `Inputs` class (as a comment above each property) whether it is **pre-populated** or **left blank/unselected on load**.

- **Pre-populated** — supplied by `Initialise_HD` (mock data) and assigned in `SetInitialInputs()`. Comment: `// Pre-populated from mock data`
- **Blank** — starts empty or unselected. Comment: `// Left blank on load`

**Dropdowns**: the options list is always supplied by `Initialise_HD`; the selected value follows the pre-populated vs blank rule.

**If the user has not specified** whether an input is pre-populated or blank, **stop and ask before generating**.

### Validation rules
For every input, also ask whether it is **required** or **optional**. **If the user has not specified, stop and ask before generating.**

`Data.InputsClass : ValidatableInputs` holds the validators. It stores the tuple shape `Func<(bool IsValid, string Label)>` (the shape inputs pass to `ValidationRef`) and also accepts `Func<ValidationResult>`. `ValidateAll()` sets `_d.I.Inputs.IsValid` and `_d.I.Inputs.FailedLabels`.

- **Required, input supports `ValidationRef`** → set `Required="true"` and `ValidationRef="_inputs.AddValidator"`. If the input also supports `ValidationUnref`, add `ValidationUnref="_inputs.RemoveValidator"`. The input registers (and removes) its own validator and shows its own inline error. **Do not** add a manual `AddValidator` for that field.
  - Ref + Unref: CurrencyInput, DatePickerInput, DateTimePicker, SingleSelectDropDownWithSearch, TextInput, TrueFalseInput, YesNoRadioButtonInput
  - Ref only: CheckBoxInput, MultiSelectDropdown, MultiSelectableListInput, NumberDropDownInput, NumberInput, PercentageInput, SelectableListInput, SingleSelectDropDown, SingleSelectDropDownTwoColumnDisplayWithSearch, TextArea, TimePickerInput
- **Required, input does not self-validate** (all `Table.*` inputs, MobileInput, MinMaxCurrencyInput_C) → register a manual validator in `SetInitialInputs()`: `_d.I.Inputs.AddValidator(() => (<condition>, nameof(_d.I.Inputs.X)));` (call `_d.I.Inputs` directly, not the inner class's own `AddValidator`)
- **Cross-field rules** (e.g. end after start) → manual validator in `SetInitialInputs()`.
- **Optional** → no `Required`, no `ValidationRef`/`ValidationUnref`, no manual validator.
- **Inputs inside a toggling `@if`** → prefer an input that supports `ValidationUnref`; flag it to the user if a Ref-only input must be used there (its validator is never removed and re-registers each time it re-renders).
- Submit flow: razor calls `_inputs.ValidateAll()` first; the `_req` method gates on `if (_d.I.Inputs.IsValid)`; `_state` and `_d.StateUpdate()` run afterwards regardless.
- Per-input extras (`CustomValidation`, `OnChangeValidation`, TextInput's `RegexValidation`) are only added when the user asks. `CustomValidation` signatures differ by input — check `inputs.md`.

### Model helpers
- `TrackedInput<T>` (`FE.Components.Domains.Utlility.Inputs.Model`) tracks initial value, history and `HasChanged`. Only use it when the user asks for change tracking.
- `ValidatableInputs` (global namespace) is the base class for every `Data.InputsClass`.

---

## Pre-Generation Plan Summary

Once all requirements are understood and all questions answered, **before writing any code**, output a plan summary in this format. The user must be able to review and confirm it before generation begins.

```
### Plan Summary

**Files**
Full paths of the .razor and .razor.cs, the namespace, and the page (if any) that will host it.

**Inner Class Methods**
Every non-boilerplate method you intend to add to each inner class of `_C`.
Format: ClassName → method signatures, one per line.
Omit classes that will only contain standard boilerplate.

Example:
- Request     → InitialRequestAsync(), SaveRowAsync()
- Inputs      → SetInitialInputs() [StartDate (pre-pop, required: DatePickerInput Ref+Unref), Amount (blank, required: TableCurrencyInput manual validator), Notes (blank, optional)]
- StateSub    → Subscribe/UnSubscribe for: SomeOther_S.Root.SelectedId
- State       → SetSelectedId()
- Action      → ShowConfirmPanel() : bool
- Switch      → GetRowCss() : string

**RC Sub-Classes**
Every sub-class inside `_RC` besides `Initialise_HD`, and what mock data each returns.

Example:
- Initialise_HD  — returns 5 mock rows and the status dropdown options
- SaveRow_HD     — pretends to save (short delay), returns success

**Mock Data**
The fields and sample values the mock will contain, and whether saves need to persist between requests (if so, where the mock store lives).

**_S State**
Every `ObservableProperty` you intend to add to `_S` sub-classes (Root or custom).
Format: SubClass.PropertyName : Type — one-line description.

Example:
- Root.SelectedId : int?  — the currently selected row ID
```

After presenting the plan, ask: **"Does this look right? Happy for me to go ahead?"**

Do not begin generating files until the user confirms.

---

## After Generating

1. **Compile check** — if the user might have the app open in Visual Studio (check `tasklist | grep -i Sandox`), **do not build inside `C:\Projects\Sandox`**. Every `dotnet build` there rewrites `obj/` (generated Razor and scoped-CSS files) under Visual Studio's feet, and a later Hot Reload in that session failed with `The attribute 'b-…' is used two or more times for this element`. Build a throwaway copy instead:
   ```bash
   S=<scratchpad>/sandox-check; rm -rf "$S"; mkdir -p "$S"
   cd /c/Projects/Sandox && tar --exclude=./bin --exclude=./obj --exclude=./node_modules --exclude=./.vs --exclude=./.claude -cf - . | (cd "$S" && tar -xf -)
   cd "$S" && dotnet build 2>&1 | grep -E "error CS|error RZ|Build succeeded"
   ```
   Delete the scratch copy afterwards. Only when the app is definitely not running may you build in place with `dotnet build 2>&1 | grep -E "error CS|Build succeeded"`. Fix any `error CS` / `error RZ` before reporting done.
2. **Tailwind** — new utility classes only appear if the CSS is rebuilt. Run `npm run build:css` (or tell the user to keep `npm run watch:css` running). To check a class exists, grep with fixed strings and the escaped form Tailwind writes: `grep -cF 'lg\:col-span-3' wwwroot/output.css`.
3. **Hosting** — a component only renders somewhere if a page includes it. If the user did not name a page, offer to create one. (A component may carry its own `@page "/route"` when the user wants it routable directly.)
4. Do not start, stop or restart the user's running app. The user may be running it from Visual Studio, which holds `bin/Debug/net8.0/Sandox.exe` open and does not always pick up edits.
5. **Checking the result in the browser without disturbing Visual Studio** — use the scratch copy from step 1 (never `dotnet build -o` from the project folder, which still rewrites the project's `obj/`) and run it on a spare port:
   ```bash
   cd "$S" && ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --urls http://localhost:5311
   ```
   Copy `wwwroot/output.css` in the copy after `npm run build:css` if new Tailwind classes were added. When finished, stop only the processes whose command line contains the scratch path or `5311` (`Get-CimInstance Win32_Process | Where-Object CommandLine -like '*sandox-check*'`), and delete the scratch folder. Never kill `Sandox.exe` from Visual Studio. Tell the user their copy needs a restart (Ctrl+F5 in Visual Studio) to see the new code.
