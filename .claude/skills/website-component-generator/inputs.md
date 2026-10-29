# Input Components Reference (Sandox)

Source of truth: `C:\Projects\Sandox\Sandox\Components\Domains\Utlility\Inputs` (namespace `FE.Components.Domains.Utlility.Inputs`). Tag names are the file names — use them exactly.

## RULE: Never use standard inputs inside a table. Never use table inputs outside a table.

## Sandox differences from the DMP platform
- **No page lock.** There is no `IgnorePageLock` parameter and no `PageLock_S`. Never generate `IgnorePageLock`. An input is disabled only by `Disabled="true"`.
- **No `AddressSearchInput`.** It was removed (it needed Google Places). Use a `TextInput` (or several) for address fields.
- Every option class is **nested** inside its input component (e.g. `SingleSelectDropDown.SingleSelectDropDownOption`).

---

## Validation — how inputs plug into the component

Most standard inputs validate themselves. You do not hand-write a required check for them.

1. Set `Required="true"` on the input.
2. Pass the component's validator list: `ValidationRef="_inputs.AddValidator"`.
3. If the input supports it, also pass `ValidationUnref="_inputs.RemoveValidator"` so its validator is removed when the input is disposed.
4. On submit, the razor calls `_inputs.ValidateAll()` and the `_req` method checks `_d.I.Inputs.IsValid`.

The input registers its own `Validate()` (signature `Func<(bool IsValid, string Label)>`) on `OnInitialized`. When it fails it also shows its own inline error.

```razor
<FE.Components.Domains.Utlility.Inputs.CurrencyInput
    @bind-Value="_d.I.Inputs.Amount"
    Label="Amount"
    Id="amount"
    Name="amount"
    Required="true"
    ValidationRef="_inputs.AddValidator"
    ValidationUnref="_inputs.RemoveValidator" />
```

| Support | Inputs |
|---|---|
| `ValidationRef` **and** `ValidationUnref` | CurrencyInput, DatePickerInput, DateTimePicker, SingleSelectDropDownWithSearch, TextInput, TrueFalseInput, YesNoRadioButtonInput |
| `ValidationRef` only | CheckBoxInput, MultiSelectDropdown, MultiSelectableListInput, NumberDropDownInput, NumberInput, PercentageInput, SelectableListInput (registers only when `Required="true"`), SingleSelectDropDown, SingleSelectDropDownTwoColumnDisplayWithSearch, TextArea, TimePickerInput |
| No self-validation | All `Table.*` inputs, MobileInput, MinMaxCurrencyInput_C |

Rules:
- **Required input with `ValidationRef`** → `Required="true"` + `ValidationRef` (+ `ValidationUnref` when supported). Do NOT also add a manual `_d.I.Inputs.AddValidator(...)` for the same field.
- **Required input without `ValidationRef`** (table inputs, MobileInput, MinMaxCurrencyInput_C) → register a manual validator in `SetInitialInputs()`.
- **Cross-field rules** (e.g. end date after start date) → manual validator in `SetInitialInputs()`.
- **Inputs inside a toggling `@if`** → prefer an input that supports `ValidationUnref`. A `ValidationRef`-only input registers again each time it is re-created and never unregisters, so stale validators pile up.
- **Optional input** → omit `Required`, `ValidationRef` and `ValidationUnref`.

### Extra per-input checks (do not use ValidationRef)
- `OnChangeValidation` — `Func<Task<(bool IsValid, string ErrorMessage)>>`, runs on every change (debounced by `OnChangeValidationDebounceMs`, default 300) and shows an inline error only. Available on CurrencyInput, DatePickerInput, TextInput.
- `CustomValidation` — runs inside `Validate()`, so it DOES affect `ValidateAll()`:
  - CurrencyInput, DatePickerInput: `Func<Task<(bool IsValid, string ErrorMessage)>>`
  - TextInput: `Func<string?>` (return an error message, or null when valid). TextInput also has `CustomValidationAsync` (`Func<Task<(bool IsValid, string ErrorMessage)>>`), `RegexValidation` + `RegexValidationMessage`, `ValidationMessage` (overrides the required message) and `MaxLength`.

---

## Parameters shared by almost every input
- `Id`, `Name`, `Label` (standard inputs only — table inputs have no visible label)
- `Disabled` — disables the input
- `Required`, `Highlight` — where listed below
- Callbacks: `AsyncCallBack` (async, after a committed change) and/or `CallBack` / `OnChange` (sync). Dropdown `AsyncCallBack`s receive the selected option: `async (v) => ...`

---

## Standard Inputs — `FE.Components.Domains.Utlility.Inputs.*`

### TextInput — `string?`
```razor
<FE.Components.Domains.Utlility.Inputs.TextInput
    @bind-Value="_d.I.Inputs.SomeText"
    Label="Some Label" Id="some-id" Name="some-name"
    Placeholder="Enter text..."
    Required="true" ValidationRef="_inputs.AddValidator" ValidationUnref="_inputs.RemoveValidator"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- `AsyncInputCallBack` / `OnInput` — every keystroke; `AsyncCallBack` / `OnChange` — blur or enter
- `DebounceMilliseconds` (0), `ReadOnly`, `Highlight`, `AutoFocus`, `MaxLength`
- Validation extras: `CustomValidation`, `CustomValidationAsync`, `RegexValidation`, `RegexValidationMessage`, `ValidationMessage`, `OnChangeValidation`, `OnChangeValidationDebounceMs`

### TextArea — `string?`
```razor
<FE.Components.Domains.Utlility.Inputs.TextArea
    @bind-Value="_d.I.Inputs.Notes"
    Label="Notes" Id="notes" Name="notes"
    Rows="4" MaxLength="500" ShowCharacterCount="true"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- `Rows` (3), `MaxLength` (0 = no limit), `MinHeight`, `ShowCharacterCount` (the counter changes to a warning state at 90% and an error state at 100%)
- `AutoFocus`, `SpellCheck` (true), `NormalizeSpacing` (tidies spacing in the displayed value)
- `OnKeyDown`, `OnBlur` event callbacks
- `Modifier` + `KeyPair` + `KeyHandler` — key-combination handler that inserts text at the cursor
- `@bind-SavedSuccessfully` — set true after a save to show a green tick; flips back to false after 3 seconds
- `Required` + `ValidationRef` (no Unref)

### NumberInput&lt;TValue&gt; — `int?` / `decimal?`
```razor
<FE.Components.Domains.Utlility.Inputs.NumberInput
    @bind-Value="_d.I.Inputs.SomeInt"
    Label="Some Label" Id="some-id" Name="some-name"
    DecimalPlaces="0" Min="0" Max="100" />
```
- `DecimalPlaces` (2), `Min`, `Max`, `Placeholder`, `Highlight`, `AutoFocus`, `Required` + `ValidationRef`

### NumberDropDownInput&lt;TValue&gt; — number box with a preset dropdown
```razor
<FE.Components.Domains.Utlility.Inputs.NumberDropDownInput
    @bind-Value="_d.I.Inputs.Term"
    Label="Term (months)" Id="term" Name="term"
    RangeStart="6" RangeEnd="60" RangeStep="6" DecimalPlaces="0" />
```
- Preset list = `RangeStart`..`RangeEnd` step `RangeStep` (defaults 1, 10, 1); free typing still allowed
- `Min`, `Max`, `DecimalPlaces` (2), `ShowBorder` (true), `MaxWidth`, `Highlight`, `Required` + `ValidationRef`

### CurrencyInput — `decimal?`
```razor
<FE.Components.Domains.Utlility.Inputs.CurrencyInput
    @bind-Value="_d.I.Inputs.SomeAmount"
    Label="Amount" Id="some-id" Name="some-name"
    CurrencySymbol="$" DecimalPlaces="2" AllowNull="true"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- `Placeholder` ("0.00"), `AllowNull` (true), `DisabledMessage`, `Highlight`, `CallBack`
- `Required` + `ValidationRef` + `ValidationUnref`, `CustomValidation`, `OnChangeValidation`, `OnChangeValidationDebounceMs`

### PercentageInput — `decimal?`
```razor
<FE.Components.Domains.Utlility.Inputs.PercentageInput
    @bind-Value="_d.I.Inputs.Rate"
    Label="Rate" Id="rate" Name="rate" DecimalPlaces="2" />
```
- `Placeholder` ("0.00"), `AllowNull` (true), `Highlight`, `CallBack`, `Required` + `ValidationRef`

### MinMaxCurrencyInput_C — two `decimal?`
```razor
<FE.Components.Domains.Utlility.Inputs.MinMaxCurrencyInput_C
    @bind-MinValue="_d.I.Inputs.MinAmount"
    @bind-MaxValue="_d.I.Inputs.MaxAmount"
    Label="Amount Range" Id="some-id" Name="some-name"
    ValidateRange="true" />
```
- `MinPlaceholder` ("Min"), `MaxPlaceholder` ("Max"), `CurrencySymbol`, `DecimalPlaces`, `AllowNull`, `ValidateRange` (true: min ≤ max)
- No `Required` / `ValidationRef` — use a manual validator if required

### MobileInput — `string?`
```razor
<FE.Components.Domains.Utlility.Inputs.MobileInput
    @bind-Value="_d.I.Inputs.MobileNumber"
    Label="Mobile" Id="some-id" Name="some-name"
    Format="MobileFormat.Australian" />
```
- Formats: `Australian` (default), `US`, `International`, `Simple`, `Minimal`
- `AutoFormat` (true), `CountryCode` ("+1"), `DebounceMilliseconds` (1000), `OnChange`
- Stores digits only, formats for display
- No `Required` / `ValidationRef` — use a manual validator if required

### CheckBoxInput (toggle) — `bool?` or `bool`
```razor
<!-- nullable -->
<FE.Components.Domains.Utlility.Inputs.CheckBoxInput
    @bind-NullBool="_d.I.Inputs.SomeFlag"
    Label="Some Label" CheckboxText="Enable this option" Id="some-id" Name="some-name" />
<!-- non-nullable -->
<FE.Components.Domains.Utlility.Inputs.CheckBoxInput
    @bind-NonNullBool="_d.I.Inputs.SomeFlag" ... />
```
- `Description`, `ShowStateIndicator` (false), `DefaultToFalse` (true), `LabelPlacement`, `Highlight`, `NoBorder`, `OnChange`
- `Required` = the toggle must be ON to pass; `ValidationRef` (no Unref)

### TrueFalseInput — `bool?`
```razor
<FE.Components.Domains.Utlility.Inputs.TrueFalseInput
    @bind-Value="_d.I.Inputs.IsActive" Label="Active" AllowNull="true" />
```
- `AllowNull` (true), `CallBack`, `Required` + `ValidationRef` + `ValidationUnref`

### YesNoRadioButtonInput — `bool?`
```razor
<FE.Components.Domains.Utlility.Inputs.YesNoRadioButtonInput
    @bind-Value="_d.I.Inputs.HasDependants"
    Label="Any dependants?" Id="deps" Name="deps"
    PositiveLabel="Yes" NegativeLabel="No" ShowClearButton="true"
    OnChange="async () => await _req.SaveAsync()" />
```
- NOTE: the async callback is named `OnChange` here (`Func<Task>`), not `AsyncCallBack`
- `Description`, `LabelPlacement` ("floating"), `Highlight`, `Required` + `ValidationRef` + `ValidationUnref`

### SelectableListInput — pick one from a list
```razor
<FE.Components.Domains.Utlility.Inputs.SelectableListInput
    @bind-Value="_d.I.Inputs.SelectedReason"
    Items="_d.I.Inputs.ReasonOptions" Label="Reason" />
```
- Option type: `SelectableListInput.SelectableListInputOption(int? id, string description)`
- `Required` + `ValidationRef` (only registers when `Required="true"`)

### MultiSelectableListInput — pick several from a list
```razor
<FE.Components.Domains.Utlility.Inputs.MultiSelectableListInput
    @bind-Value="_d.I.Inputs.SelectedItems"
    Items="_d.I.Inputs.ItemOptions" Label="Items" MinSelections="1" MaxSelections="3" />
```
- Option type: `MultiSelectableListInput.MultiSelectableListInputOption(int? id, string description, string? subtitle = null)`
- `@bind-Value` is a `List<MultiSelectableListInputOption>`; `Required` + `ValidationRef`

### SingleSelectDropDown
```razor
<FE.Components.Domains.Utlility.Inputs.SingleSelectDropDown
    @bind-SelectedOption="_d.I.Inputs.SomeOption"
    Options="_d.I.Inputs.SomeOptions"
    Label="Some Label" Id="some-id" Name="some-name"
    Placeholder="Select an option..." AllowReset="true"
    AsyncCallBack="async (v) => await _req.SaveAsync()" />
```
- Option type: `SingleSelectDropDown.SingleSelectDropDownOption(string value, int? id = null)`
- `AllowReset`, `HighlightWhenSelected`, `AutoOpen`, `OnChange`, `CallBack`, `Required` + `ValidationRef`

### SingleSelectDropDownWithSearch
- Same parameters as SingleSelectDropDown, plus `Highlight`
- Option type: `SingleSelectDropDownWithSearch.SingleSelectDropDownWithSearchOption(string value, int? id = null)`
- `Required` + `ValidationRef` + `ValidationUnref`

### SingleSelectDropDownTwoColumnDisplayWithSearch
```razor
<FE.Components.Domains.Utlility.Inputs.SingleSelectDropDownTwoColumnDisplayWithSearch
    @bind-SelectedOption="_d.I.Inputs.SomeOption"
    Options="_d.I.Inputs.SomeOptions"
    Label="Some Label" Id="some-id" Name="some-name"
    ShowSecondaryValueInInput="true" MaxVisibleItems="100" AllowReset="true" />
```
- Option type: `SingleSelectDropDownTwoColumnDisplayWithSearch.SingleSelectDropDownTwoColumnDisplayWithSearchOption(string value, string? secondaryValue = null, int? id = null)`
- Searches both columns; `MaxVisibleItems` caps rendered rows ("X more results...")
- `DropdownMaxWidth`, `AutoOpen`, `HighlightWhenSelected`, `Required` + `ValidationRef`

### MultiSelectDropdown
```razor
<FE.Components.Domains.Utlility.Inputs.MultiSelectDropdown
    @bind-SelectedOptions="_d.I.Inputs.SelectedTags"
    Options="_d.I.Inputs.TagOptions"
    Label="Tags" Id="tags" Name="tags" />
```
- Option type: `MultiSelectDropdown.MultiSelectDropDownOption(string value, int? id = null)`
- `CloseOnOutsideClick` (true), `CloseOnEscape` (true), `OnChange`; `AsyncCallBack` receives the list
- `Required` + `ValidationRef`

### DatePickerInput — date only, `DateTime?`
```razor
<FE.Components.Domains.Utlility.Inputs.DatePickerInput
    @bind-Value="_d.I.Inputs.StartDate"
    Label="Start date" Id="start" Name="start"
    MinDate="DateTime.Today" FutureDatesOnly="false"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- `DateFormat` ("dd/MM/yyyy"), `Placeholder`, `MinDate`, `MaxDate`, `FutureDatesOnly`, `ReadOnly`, `Highlight`, `CallBack`
- `Required` + `ValidationRef` + `ValidationUnref`, `CustomValidation`, `OnChangeValidation`

### DateTimePicker — date with optional time, `DateTime?`
```razor
<FE.Components.Domains.Utlility.Inputs.DateTimePicker
    @bind-Value="_d.I.Inputs.AppointmentAt"
    Label="Appointment" Id="appt"
    IncludeTime="true" Use12HourFormat="false" />
```
- `IncludeTime` adds the time picker and a "Now" button; format auto-switches to `dd/MM/yyyy HH:mm` (or `hh:mm tt` in 12-hour)
- `MinDate`, `MaxDate`, `ReadOnly`, `CallBack`, `Required` + `ValidationRef` + `ValidationUnref`
- No `Name` parameter

### TimePickerInput — `TimeSpan?`
```razor
<FE.Components.Domains.Utlility.Inputs.TimePickerInput
    @bind-Value="_d.I.Inputs.CallTime" Label="Call time" Use12HourFormat="true" />
```
- `ReadOnly`, `CallBack`, `Required` + `ValidationRef`

---

## Table Inputs — `FE.Components.Domains.Utlility.Inputs.Table.*`

Use inside tables only. No visible label. **None of them self-validate** — required table fields need a manual validator.

### Table.TableTextInput — `string?`
- `MaxLength`, `TrimWhitespace` (true), `Placeholder`, `OnChange`, `AsyncCallBack`

### Table.TableCurrencyInput — `decimal?`
- `CurrencySymbol`, `DecimalPlaces`, `Placeholder`, `OnChange`, `AsyncCallBack`
- Borderless, fills the cell; no `AllowNull` (empty always becomes null)

### Table.TablePercentageInput — `decimal?`
- `DecimalPlaces`, `AllowNull`, `Placeholder`, `CallBack`, `AsyncCallBack`; clamps 0–100, `%` on the right

### Table.TableCheckBoxInput / Table.TableCheckBoxInput2 — `bool?` or `bool`
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TableCheckBoxInput
    @bind-NullBool="row.SomeFlag" Id="some-id" Name="some-name" />
```
- Bind with `@bind-NullBool` or `@bind-NonNullBool` (auto-detected)
- Both are toggles with the same parameters; `TableCheckBoxInput2` shows `Label` as a small uppercase heading above the toggle

### Table.TableSingleSelectDropDown (no search)
- Option type: `TableSingleSelectDropDown.TableSingleSelectDropDownOption(string? value, int? id = null)`
- `Placeholder` ("Select..."), `OnChange`, `AsyncCallBack` receives the option
- Dropdown is `absolute` positioned (stays in the cell's DOM)

### Table.TableSingleDropDownWithSearch
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TableSingleDropDownWithSearch
    @bind-SelectedOption="row.SomeOption"
    Options="_d.I.Inputs.XxxOptions"
    Id="some-id" Name="some-name" EnableSearch="true" />
```
- Option type: `TableSingleDropDownWithSearch.TableSingleDropDownWithSearchOption(string? value, int? id = null)`
- Attaches to `document.body` to escape cell clipping; highlights matches; `EnableSearch="false"` shows a "Search options..." button instead

---

## Model helpers (`FE.Components.Domains.Utlility.Inputs.Model`)

### TrackedInput&lt;T&gt;
Wraps a value and remembers where it started. Not wired into any input component.
- `TrackedValue` — the first assignment also sets `InitialValue`; later assignments push the old value into `History`
- `InitialValue`, `History`, `PreviousValue`, `HasChanged` (false if changed and changed back), `HasInitialValue`
- `Reset()` — current value becomes the new initial value; history cleared
- Suits pre-populated inputs where you need "has the user changed this?"

### ValidatableInputs (abstract) + ValidationResult
`Data.InputsClass` inherits `ValidatableInputs` (see `codebehind.md`). It stores validators in the same tuple shape the input components use, `Func<(bool IsValid, string Label)>`, so `ValidationRef="_inputs.AddValidator"` works directly.

- `AddValidator` / `RemoveValidator` — overloads for the tuple shape and for `Func<ValidationResult>` (adapted internally, and removable by the same delegate)
- `ValidateAll()` — runs every validator, sets and returns `IsValid` and `FailedLabels`
- `IsValid` — any failure makes it false, including an unlabelled one
- `FailedLabels` — labels of failed validators (unlabelled failures are left out of the list)

`ValidationResult` is `record ValidationResult(bool IsValid, string? Label = null)` in `FE.Components.Shared.Validation`. Prefer the tuple for new validators.

---

## AsyncCallBack in Inputs

Can be a multi-line lambda:

```razor
AsyncCallBack="async () => {
    await _req.SaveValuesAsync();
    _state.UpdateTotal();
    await _d.StateUpdate();
}"
```

Dropdown callbacks receive the selected option:
```razor
AsyncCallBack="async (v) => { await _req.SaveValuesAsync(); }"
```
