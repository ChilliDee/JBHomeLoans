---
name: "website-component-generator"
description: Generates Blazor component files (.razor + .razor.cs with the _C, _RC and _S classes) for the Sandox project, following the DMP component pattern but with no database, no BusinessLogic package and no page lock. Use for any new Sandox component, code-behind, razor, RC sub-class or _S state work.
---

# Sandox Component Generator

Generates complete Blazor component files for the **Sandox** project (`C:\Projects\Sandox\Sandox`, the folder holding `Sandox.csproj`): a standalone Blazor Server environment for building CRM components without the CRM backend.

It is based on the `blazor-component-generator` skill (the DMP platform version) and keeps the same architecture. What differs is the environment:

| DMP platform | Sandox |
|---|---|
| `BusinessLogic.Core` NuGet package | Not referenced, and never to be added back. The pattern's base classes (`ST_Subscriptions`, `ST_DTOResponse`, `ObservableProperty`, `StateChangeContext`, `InterceptorErrorHandlingFactory`) are local files in `Framework/` with the same `Mortgage.State` / `Mortgage.Interceptors...` namespaces. They are Sandox's own versions with the same names and methods, not copies of the package's code |
| `_RC` requests read from `IUnitOfWork` | `_RC` requests build **mock data in memory** |
| Errors go to `IFrontEndErrorLog_B` (DB + Teams) | Errors go to the console log (`ILogger`) through the interceptor; `_RC` calls `SetRequestFailed(ex.Message)` |
| `CoreException` / `FrontEndException` | Do not exist. Catch `Exception` only |
| Inputs disabled by `PageLock_S`, `IgnorePageLock` parameter | Page lock removed. Never generate `IgnorePageLock`. An input is disabled only by `Disabled` |
| `AddressSearchInput` (Google Places) | Deleted. Do not generate it |
| Auth, SignalR, DB contexts | None |

Given a component name (e.g. `Home_P_C` for a page, `Home_Sections_Calculator_C` for one of its sections), always generate **two files**:
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

### How to ask questions

**Every question to the user goes through the `AskUserQuestion` tool** (the clickable question format), never as plain text in the reply. This covers all the questions in this skill: component name, page and operation, pre-populated vs blank, required vs optional, the plan confirmation, and the delete confirmation for C: Removing.

- Group related questions into one call (up to 4 questions per call) rather than asking one at a time.
- Give concrete options (e.g. `A: Editing` / `B: Creating` / `C: Removing`; `Pre-populated` / `Left blank`; `Required` / `Optional`). The user can always pick "Other" to type a free answer, so use that for names.
- When there are more inputs than fit in one call, ask in batches, or use `multiSelect` (e.g. "Which of these inputs are required?").

### Which page, and which operation (required)

Every build must start with the user stating **which page** the work is on and **which operation** it is. If either is missing, **stop and ask before doing anything else**. Do not infer the page from the component name, from the files that exist, or from an earlier request in the conversation.

| Operation | Meaning |
|---|---|
| **A: Editing** | The page already exists. Change it or its sub components (including adding a new sub component to it). |
| **B: Creating** | The page does not exist yet. Create its area folder, `<Area>_P_C`, and the `Navigation`, `Sections` and `Footer` groups (each with its `A`), plus any sections asked for. |
| **C: Removing** | Delete the page, or a named sub component of it. |

**Page name** = the page's URL name: what appears in the address bar for that page, which is the value in its `@page` directive without the leading slash (`@page "/demo"` → page name `demo`, area folder `Pages/Demo/`). The user may give it in that URL form; map it to the area folder by matching the `@page` route, not by guessing the casing.

When it is missing, ask with `AskUserQuestion`: one question for the operation (options `A: Editing`, `B: Creating`, `C: Removing`) and one for the page (options = the existing page names found from the `@page` routes; the user types a new name under "Other" when creating).

Check the answer against the project before planning:
- **A: Editing** — a `_P_C` with that `@page` route must exist. If none does, say so and ask whether they meant B.
- **B: Creating** — no `_P_C` may already use that route (routes must be unique). If one does, say so and ask whether they meant A.
- **C: Removing** — the page must exist. List every file and folder that would be deleted, and every other file that references them (`@using` lines, tags, injected `_S` classes, links to the route), and **wait for the user to confirm before deleting anything**. Removing a sub component also means removing its tag and `@using` from the parent.

> ⚠️ **BOILERPLATE REMINDER — READ THIS BEFORE EVERY GENERATION**
> Output the **complete** code-behind and razor files using the full templates from `codebehind.md` and `razor.md`. Every inner class, every lifecycle method, and the `_S` class must be present in full, even if empty. Do not abbreviate or replace any section with a comment like `// ... rest of boilerplate`. No exceptions, however simple the component.

---

## Where files go and how to name them

Folder and file naming follows the page tree. Never break these rules.

Every page has its own `A` (the page component) and exactly three group folders beside it: `Navigation`, `Sections` and `Footer`. Each group has its own `A` holding the group's component, and the group's sub components are folders beside that `A` (siblings), each with its own `A`.

```
Components/Domains/Pages/
  <Area>/                                   area folder, e.g. Home
    A/
      <Area>_P_C.razor(.cs)                 the PAGE: area + _P. Owns @page "/route". A thin shell.
    Navigation/
      A/
        <Area>_Navigation_C.razor(.cs)      the header / nav bar
      <Child>/A/
        <Area>_Navigation_<Child>_C         a sub component of the navigation
    Sections/
      A/
        <Area>_Sections_C.razor(.cs)        the container: places the sections in order
      <Section>/A/
        <Area>_Sections_<Section>_C         one section of the page
      <Section>/<Child>/A/
        <Area>_Sections_<Section>_<Child>_C going deeper: a sibling of the section's A, with its own A
    Footer/
      A/
        <Area>_Footer_C.razor(.cs)          the footer
      <Child>/A/
        <Area>_Footer_<Child>_C
```

Worked example (the `Home` page, which exists in the project):

| Folder | Component |
|---|---|
| `Pages/Home/A/` | `Home_P_C` (page, `@page "/home"`) |
| `Pages/Home/Navigation/A/` | `Home_Navigation_C` |
| `Pages/Home/Sections/A/` | `Home_Sections_C` |
| `Pages/Home/Sections/Hero/A/` | `Home_Sections_Hero_C` |
| `Pages/Home/Sections/Calculator/A/` | `Home_Sections_Calculator_C` |
| `Pages/Home/Footer/A/` | `Home_Footer_C` |
| `Pages/Home/Sections/Calculator/Card/A/` | `Home_Sections_Calculator_Card_C` (how a deeper one would be named) |

Rules:
- **Every component sits in an `A` folder.** Sub component folders are siblings of the parent's `A`.
- **Page** = `<Area>_P_C`. It has `_P`, carries no page name, and is the only component with `@page`. One page per area folder; a second page needs its own area folder. The page is a thin shell: its markup is only `<Area>_Navigation_C`, `<Area>_Sections_C` and `<Area>_Footer_C`, in that order. No header, section or footer markup goes in the page itself.
- **Navigation, Sections, Footer** always exist for a page, each with its own `A`. Create all three when creating a page, even if one starts out nearly empty.
- **`<Area>_Sections_C` is a container.** It wraps each section in `<section id="..." class="scroll-mt-...">` (the ids are the anchor targets used by the navigation) and holds no content of its own.
- **A new piece of page content is a new folder under `Sections/`.** Nothing is added beside `Navigation`, `Sections` and `Footer` at the page level.
- **Names follow the folders.** Sub components have no `_P`. The file name carries the full tree from the area down, including the group (`Home_Sections_Calculator_C`, never `Home_Calculator_C`), while the folder is only the component's own name (`Calculator/`).
- Each component's `_RC` and `_S` take the same stem: `Home_Sections_Calculator_RC`, `Home_Sections_Calculator_S`.
- The folder path becomes the namespace, prefixed `FE.` (`Pages/Home/Sections/Calculator/A/` → `namespace FE.Components.Domains.Pages.Home.Sections.Calculator.A;`).
- A parent hosts its children by tag with a `@using` for the child namespace (`@using FE.Components.Domains.Pages.Home.Sections.Hero.A` then `<Home_Sections_Hero_C />`).
- Components that repeat (a card per row) get a unique root id, e.g. `id="@($"{_d.I.ClassName}-{Item.Id}")"`, and `@key` on the tag. They take `[Parameter]`s from the parent; the parent passes callbacks (`EventCallback<T>`) rather than the child writing the parent's state.
- Shared state between a page and its children: the page's `_S` holds it. A child injects the page's `_S` in its `Dependencies` (wiring), and subscribes only when it needs to re-render on change.
- The page component itself is routable (`@page` in its own `.razor`). There is no separate thin page file under `Components/Pages/`.
- Every `*_S` class in a `FE.Components*` namespace is registered as scoped automatically by `Program.cs`. **Never add a manual `AddScoped` for an `_S` class.**
- The `Pages/Home` tree is the reference example for **structure only** (folders, names, component split, data flow). Its colours, fonts and sizes belong to one site and must not be carried into another. `Pages/Demo/A/Demo_P_List_C` is older and does not follow these rules; do not copy its name, its layout or its validation code.

### Styling

This skill is a structural blueprint shared by sites that each have their own look. It deliberately contains **no styling**: no colours, fonts, type sizes, spacing scale, corner radius, shadows or brand.

- **Where the look comes from:** the site's brief (Part 2, Brand). Every site defines its own palette and style there.
- **Placeholders in the templates:** wherever the templates in `razor.md` and `codebehind.md` would carry styling, they show `{style: <role>}` (for example `{style: loading message}`, `{style: error box}`, `{style: primary button}`). Replace each one with the site's own Tailwind classes for that role.
- **Which colour each role uses:** the brief's Part 2 has a palette (Main, Accent, Dark, Tint, Grey, Error, Success, White) and a **Colour roles** table whose role names match the placeholders. Build each role from its row: the palette colour named for background, text and border, plus the brief's heading style, button shape and corner choices. Blank cells are not guesses to make silently: propose them in the plan summary (a short "Colour roles proposed" list) for the user to approve. Once a site has pages, its existing components are the record of the agreed mapping. **Never output a `{style: ...}` placeholder literally.** Classes that only control layout or behaviour (`flex`, `gap-3`, `sticky top-0`, `fixed inset-0`, `animate-spin`, icon `w-4 h-4`) are kept as written.
- **Consistency within a site:** once a site has pages, match its existing components so every page of that site looks the same.
- **Site accent for the shared components:** the shared components in `Components/Domains/Utlility` (inputs, dropdowns, date pickers, modals, pagination, graphs, splitters) never use a fixed colour. They use the `accent-*` scale (`bg-accent-600`, `ring-accent-500`, `text-accent-700`...), defined in `wwwroot/input.css` with indigo as the default. A site sets its accent once: add a `.site-<site-name>` class to `input.css` holding all eleven `--color-accent-50` … `--color-accent-950` values (from the brief's accent colour), and put that class on the root element of every one of that site's `<Area>_P_C` pages. When creating a site's first page, add its class; when adding a page to an existing site, reuse it. Never write a fixed colour such as `indigo-600` or `blue-500` into a shared component; use `accent-*`. (JJ's Vehicle Finance uses `.site-jjs-vehicle-finance`.)
- **No look defined yet:** if the site's style is not known (no brief Part 2 and no existing pages for that site), ask with `AskUserQuestion` before generating. Do not invent a palette and do not fall back on another site's.

After changing any markup, rebuild the CSS (`npm run build:css`) and hard-refresh the browser: `output.css` is cached, and a stale copy makes new classes look like they are missing.
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
6. **Loading gate** — always combine `InitialisationCompleted` and `RequestIsLoading` in the same `@if` block.
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

**Page**
The page name (URL form, e.g. `demo`) and the operation: A: Editing, B: Creating or C: Removing.

**Files**
Full paths of the .razor and .razor.cs, the namespace, and the page that will host it. For C: Removing, the files and folders to delete and the references to clean up instead.

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

**Colour roles proposed** (only when the brief left any blank and the site has no existing pages that settle them)
Role → palette colour for background / text / border, one per line. Omit this block when nothing needs proposing.

**_S State**
Every `ObservableProperty` you intend to add to `_S` sub-classes (Root or custom).
Format: SubClass.PropertyName : Type — one-line description.

Example:
- Root.SelectedId : int?  — the currently selected row ID
```

After presenting the plan, ask with `AskUserQuestion`: **"Does this look right? Happy for me to go ahead?"** (options: `Go ahead` / `Change something`).

Do not begin generating files until the user confirms.

---

## After Generating

1. **Compile check** — if the user might have the app open in Visual Studio (check `tasklist | grep -i Sandox`), **do not build inside `C:\Projects\Sandox\Sandox`**. Every `dotnet build` there rewrites `obj/` (generated Razor and scoped-CSS files) under Visual Studio's feet, and a later Hot Reload in that session failed with `The attribute 'b-…' is used two or more times for this element`. Build a throwaway copy instead:
   ```bash
   S=<scratchpad>/sandox-check; rm -rf "$S"; mkdir -p "$S"
   cd /c/Projects/Sandox/Sandox && tar --exclude=./bin --exclude=./obj --exclude=./node_modules --exclude=./.vs --exclude=./.claude -cf - . | (cd "$S" && tar -xf -)
   cd "$S" && dotnet build 2>&1 | grep -E "error CS|error RZ|Build succeeded"
   ```
   Delete the scratch copy afterwards. Only when the app is definitely not running may you build in place with `dotnet build 2>&1 | grep -E "error CS|Build succeeded"`. Fix any `error CS` / `error RZ` before reporting done.
2. **Tailwind** — new utility classes only appear if the CSS is rebuilt. Run `npm run build:css` (or tell the user to keep `npm run watch:css` running). To check a class exists, grep with fixed strings and the escaped form Tailwind writes: `grep -cF 'lg\:col-span-3' wwwroot/output.css`.
3. **Hosting** — a component only renders somewhere if a page includes it. The page is always known by this point (see "Which page, and which operation"). (A component may carry its own `@page "/route"` when the user wants it routable directly.)
4. Do not start, stop or restart the user's running app. The user may be running it from Visual Studio, which holds `bin/Debug/net8.0/Sandox.exe` open and does not always pick up edits.
5. **Checking the result in the browser without disturbing Visual Studio** — use the scratch copy from step 1 (never `dotnet build -o` from the project folder, which still rewrites the project's `obj/`) and run it on a spare port:
   ```bash
   cd "$S" && ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --urls http://localhost:5311
   ```
   Copy `wwwroot/output.css` in the copy after `npm run build:css` if new Tailwind classes were added. When finished, stop only the processes whose command line contains the scratch path or `5311` (`Get-CimInstance Win32_Process | Where-Object CommandLine -like '*sandox-check*'`), and delete the scratch folder. Never kill `Sandox.exe` from Visual Studio. Tell the user their copy needs a restart (Ctrl+F5 in Visual Studio) to see the new code.
