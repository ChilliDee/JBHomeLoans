# Input Components Reference (Sandox)

Generated from the components that actually exist in `Components/Domains/Utlility/Inputs/`. The component name is the razor file name. If a component isn't listed here, it doesn't exist in Sandox — don't use it.

## RULE: Never use standard inputs inside a table. Never use table inputs outside a table.

- Standard inputs: `FE.Components.Domains.Utlility.Inputs.<Name>` (folder `Inputs/`)
- Table inputs: `FE.Components.Domains.Utlility.Inputs.Table.<Name>` (folder `Inputs/Table/`)
- Table inputs have **no `Label` parameter** (except `TablePercentageInput` and `TableCheckBoxInput`, which still declare one).

---

## Nested option types and enums

Option classes (`SingleSelectDropDownOption` etc.) and enums (`MobileFormat`, `KeyModifier`) are declared **inside** their component, so they are nested types (`SingleSelectDropDown.SingleSelectDropDownOption`). To use the short name in a code-behind, add a `using static` for the component:

```csharp
using static FE.Components.Domains.Utlility.Inputs.SingleSelectDropDown;
using static FE.Components.Domains.Utlility.Inputs.Table.TableSingleSelectDropDown;
```

If two `using static` imports would expose the same short name, or you'd rather not, qualify it: `SingleSelectDropDown.SingleSelectDropDownOption`.

In razor, qualify enums: `Format="FE.Components.Domains.Utlility.Inputs.MobileInput.MobileFormat.Australian"`.

| Option type | Declared in | Constructor |
|---|---|---|
| `SingleSelectDropDownOption` | `SingleSelectDropDown` | `(string value, int? id = null)` |
| `SingleSelectDropDownWithSearchOption` | `SingleSelectDropDownWithSearch` | `(string value, int? id = null)` |
| `SingleSelectDropDownTwoColumnDisplayWithSearchOption` | `SingleSelectDropDownTwoColumnDisplayWithSearch` | `(string value, string? secondaryValue = null, int? id = null)` |
| `MultiSelectDropDownOption` | `MultiSelectDropdown` | `(string value, int? id = null)` |
| `SelectableListInputOption` | `SelectableListInput` | `(int? id, string description)` |
| `MultiSelectableListInputOption` | `MultiSelectableListInput` | `(int? id, string description, string? subtitle = null)` |
| `TableSingleSelectDropDownOption` | `Table.TableSingleSelectDropDown` | `(string? value, int? id = null)` |
| `TableSingleDropDownWithSearchOption` | `Table.TableSingleDropDownWithSearch` | `(string? value, int? id = null)` |

---

## Callback conventions

- `AsyncCallBack` is `Func<Task>?` on most inputs (text, currency, number, checkbox, etc.).
- On **dropdowns and `DatePickerInput` / `DateTimePicker`** `AsyncCallBack` is an `EventCallback` (dropdowns: `EventCallback<TOption?>`). A lambda works the same: `AsyncCallBack="async (v) => { ... }"` / `AsyncCallBack="async () => { ... }"`.
- `OnChange` / `CallBack` are the sync variants where present. `YesNoRadioButtonInput` has only `OnChange` (`Func<Task>?`).
- Multi-line lambdas are fine:

```razor
AsyncCallBack="async () => {
    await _req.SaveValuesAsync();
    _state.UpdateTotal();
    await _d.StateUpdate();
}"
```

## Built-in `Required` and `ValidationRef`

Most standard inputs also have `Required` (bool) and `ValidationRef` (`Action<Func<(bool IsValid, string Label)>>?`), which let the input register its own validator. The generated components use the `_inputs.AddValidator(...)` pattern from `codebehind.md` instead. Only use `Required="true"` for the visual required state if the user asks for it, and don't wire `ValidationRef` unless asked.

---

## Standard Inputs

### TextInput
```razor
<FE.Components.Domains.Utlility.Inputs.TextInput
    @bind-Value="_d.I.Inputs.SomeText"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    Placeholder="Enter text..."
    MaxLength="100"
    AsyncInputCallBack="async () => { /* fires on every keystroke */ }"
    AsyncCallBack="async () => { /* fires on blur/enter */ }" />
```
- Value: `string?`. Other params: `OnInput`, `OnChange` (sync), `Disabled`, `ReadOnly`, `Highlight`, `DebounceMilliseconds`, `AutoFocus`, `RegexValidation` / `RegexValidationMessage`, `ValidationMessage`.

### TextArea
```razor
<FE.Components.Domains.Utlility.Inputs.TextArea
    @bind-Value="_d.I.Inputs.SomeText"
    Label="Notes"
    Id="some-id"
    Name="some-name"
    Rows="4"
    MaxLength="500"
    ShowCharacterCount="true"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `string?`. `Rows` (default 3), `MaxLength` (0 = none), `MinHeight`, `AutoFocus`, `SpellCheck`, `Disabled`, `NormalizeSpacing`.
- Key combo handler: `Modifier` (`KeyModifier.None/Ctrl/Shift/Alt/Meta`), `KeyPair`, `KeyHandler` (`Func<Task<string?>>`).

### NumberInput (generic — `TValue`, e.g. `int?` / `decimal?`)
```razor
<FE.Components.Domains.Utlility.Inputs.NumberInput
    @bind-Value="_d.I.Inputs.SomeInt"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    DecimalPlaces="0"
    Min="0" />
```
- Also: `Max`, `Placeholder`, `Highlight`, `AutoFocus`, `Disabled`, `OnChange`.

### NumberDropDownInput (generic — `TValue`)
```razor
<FE.Components.Domains.Utlility.Inputs.NumberDropDownInput
    @bind-Value="_d.I.Inputs.SomeInt"
    Label="Term (years)"
    Id="some-id"
    Name="some-name"
    RangeStart="1" RangeEnd="30" RangeStep="1" />
```
- Also: `Min`, `Max`, `DecimalPlaces`, `Placeholder`, `ShowBorder`, `MaxWidth`, `Disabled`.

### CurrencyInput
```razor
<FE.Components.Domains.Utlility.Inputs.CurrencyInput
    @bind-Value="_d.I.Inputs.SomeAmount"
    Label="Amount"
    Id="some-id"
    Name="some-name"
    CurrencySymbol="$"
    DecimalPlaces="2"
    AllowNull="true"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `decimal?`. Also `CallBack`, `Placeholder`, `Highlight`, `Disabled`, `DisabledMessage`.

### PercentageInput
```razor
<FE.Components.Domains.Utlility.Inputs.PercentageInput
    @bind-Value="_d.I.Inputs.SomePercentage"
    Label="Rate"
    Id="some-id"
    Name="some-name"
    DecimalPlaces="2"
    AllowNull="true" />
```
- Value: `decimal?`. Also `CallBack`, `AsyncCallBack`, `Placeholder`, `Highlight`, `Disabled`.

### MinMaxCurrencyInput_C
Note the real component name ends in `_C`.
```razor
<FE.Components.Domains.Utlility.Inputs.MinMaxCurrencyInput_C
    @bind-MinValue="_d.I.Inputs.MinAmount"
    @bind-MaxValue="_d.I.Inputs.MaxAmount"
    Label="Amount Range"
    Id="some-id"
    Name="some-name"
    ValidateRange="true" />
```
- Values: `decimal?`. Also `MinPlaceholder`, `MaxPlaceholder`, `CurrencySymbol`, `DecimalPlaces`, `AllowNull`, `Disabled`, `AsyncCallBack`, `CallBack`.

### MobileInput
```razor
<FE.Components.Domains.Utlility.Inputs.MobileInput
    @bind-Value="_d.I.Inputs.MobileNumber"
    Label="Mobile"
    Id="some-id"
    Name="some-name"
    Format="FE.Components.Domains.Utlility.Inputs.MobileInput.MobileFormat.Australian" />
```
- `MobileFormat`: `US`, `International`, `Simple`, `Minimal`, `Australian`. Value: `string?`. Also `CountryCode`, `AutoFormat`, `DebounceMilliseconds`, `Disabled`, `OnChange`, `AsyncCallBack`.

### CheckBoxInput (supports both `bool` and `bool?`)
```razor
<!-- non-nullable -->
<FE.Components.Domains.Utlility.Inputs.CheckBoxInput
    @bind-NonNullBool="_d.I.Inputs.SomeFlag"
    Label="Some Label"
    CheckboxText="Enable this option"
    Id="some-id"
    Name="some-name" />

<!-- nullable -->
<FE.Components.Domains.Utlility.Inputs.CheckBoxInput
    @bind-NullBool="_d.I.Inputs.SomeNullableFlag"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    DefaultToFalse="true" />
```
- Also `Description`, `ShowStateIndicator`, `LabelPlacement`, `Highlight`, `NoBorder`, `Disabled`, `OnChange` (`Action<bool?>`), `AsyncCallBack`.

### TrueFalseInput
```razor
<FE.Components.Domains.Utlility.Inputs.TrueFalseInput
    @bind-Value="_d.I.Inputs.SomeFlag"
    Label="Some Label"
    AllowNull="true" />
```
- Value: `bool?`. Also `CallBack`, `AsyncCallBack`, `Disabled`.

### YesNoRadioButtonInput
```razor
<FE.Components.Domains.Utlility.Inputs.YesNoRadioButtonInput
    @bind-Value="_d.I.Inputs.SomeChoice"
    Label="Some Question"
    Id="some-id"
    Name="some-name"
    PositiveLabel="Yes"
    NegativeLabel="No"
    ShowClearButton="true" />
```
- Value: `bool?`. Also `Description`, `LabelPlacement`, `Highlight`, `Disabled`, `OnChange` (`Func<Task>?`).

### SingleSelectDropDown
```razor
<FE.Components.Domains.Utlility.Inputs.SingleSelectDropDown
    @bind-SelectedOption="_d.I.Inputs.SomeOption"
    Options="_d.I.Inputs.SomeOptions"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    Placeholder="Select an option..."
    AllowReset="true"
    HighlightWhenSelected="false"
    AsyncCallBack="async (v) => await _req.SaveAsync()" />
```
- Options: `IEnumerable<SingleSelectDropDownOption>`. Also `AutoOpen`, `Disabled`, `OnChange`, `CallBack`.

### SingleSelectDropDownWithSearch
```razor
<FE.Components.Domains.Utlility.Inputs.SingleSelectDropDownWithSearch
    @bind-SelectedOption="_d.I.Inputs.SomeOption"
    Options="_d.I.Inputs.SomeOptions"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    AllowReset="true"
    AsyncCallBack="async (v) => await _req.SaveAsync()" />
```
- Options: `IEnumerable<SingleSelectDropDownWithSearchOption>`. Also `Highlight`, `HighlightWhenSelected`, `AutoOpen`, `Disabled`.

### SingleSelectDropDownTwoColumnDisplayWithSearch
```razor
<FE.Components.Domains.Utlility.Inputs.SingleSelectDropDownTwoColumnDisplayWithSearch
    @bind-SelectedOption="_d.I.Inputs.SomeOption"
    Options="_d.I.Inputs.SomeOptions"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    ShowSecondaryValueInInput="true"
    MaxVisibleItems="100"
    AllowReset="true"
    AsyncCallBack="async (v) => await _req.SaveAsync()" />
```
- Options: `IEnumerable<SingleSelectDropDownTwoColumnDisplayWithSearchOption>`. Primary + secondary column; searches both. `MaxVisibleItems` caps rendered items. Also `DropdownMaxWidth`.

### MultiSelectDropdown
```razor
<FE.Components.Domains.Utlility.Inputs.MultiSelectDropdown
    @bind-SelectedOptions="_d.I.Inputs.SomeSelected"
    Options="_d.I.Inputs.SomeOptions"
    Label="Some Label"
    Id="some-id"
    Name="some-name"
    Placeholder="Select..." />
```
- `SelectedOptions`: `List<MultiSelectDropDownOption>?`. `AsyncCallBack` is `EventCallback<List<MultiSelectDropDownOption>?>`. Also `CloseOnOutsideClick`, `CloseOnEscape`, `OnChange`, `Disabled`.

### SelectableListInput
```razor
<FE.Components.Domains.Utlility.Inputs.SelectableListInput
    Items="_d.I.Inputs.SomeItems"
    @bind-Value="_d.I.Inputs.SomeSelected"
    Label="Some Label" />
```
- `Items`: `List<SelectableListInputOption>`; `Value`: `SelectableListInputOption?`.

### MultiSelectableListInput
```razor
<FE.Components.Domains.Utlility.Inputs.MultiSelectableListInput
    Items="_d.I.Inputs.SomeItems"
    @bind-Value="_d.I.Inputs.SomeSelected"
    Label="Some Label"
    MinSelections="1"
    MaxSelections="3" />
```
- `Items` / `Value`: `List<MultiSelectableListInputOption>`.

### DatePickerInput
```razor
<FE.Components.Domains.Utlility.Inputs.DatePickerInput
    @bind-Value="_d.I.Inputs.SomeDate"
    Label="Date"
    Id="some-id"
    Name="some-name"
    Placeholder="dd/MM/yyyy"
    DateFormat="dd/MM/yyyy"
    MinDate="DateTime.Today"
    FutureDatesOnly="false"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `DateTime?`. Also `MaxDate`, `ReadOnly`, `Disabled`, `Highlight`, `CallBack`. **No time picker on this component** — use `DateTimePicker` if time is needed. Sandox has no page-lock state, so there is no `IgnorePageLock`.

### DateTimePicker
```razor
<FE.Components.Domains.Utlility.Inputs.DateTimePicker
    @bind-Value="_d.I.Inputs.SomeDateTime"
    Label="Date and time"
    Id="some-id"
    IncludeTime="true"
    Use12HourFormat="false"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `DateTime?`. Also `Placeholder`, `DateFormat`, `MinDate`, `MaxDate`, `ReadOnly`, `Disabled`, `CallBack`.

### TimePickerInput
```razor
<FE.Components.Domains.Utlility.Inputs.TimePickerInput
    @bind-Value="_d.I.Inputs.SomeTime"
    Label="Time"
    Use12HourFormat="false" />
```
- Value: `TimeSpan?`. Also `ReadOnly`, `Disabled`, `CallBack`, `AsyncCallBack`.

### AddressSearchInput
Only parameter is `OnResultSelectedAsync` (`EventCallback<AddressSearchInputSelectedResult>`). Read `AddressSearchInput.razor` before using it — it does not follow the same `Value`/`Label` conventions.

---

## Table Inputs — `FE.Components.Domains.Utlility.Inputs.Table.*`

Use inside tables only. Bind to row properties.

### Table.TableTextInput
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TableTextInput
    @bind-Value="row.SomeText"
    Id="some-id"
    Name="some-name"
    MaxLength="100"
    TrimWhitespace="true"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `string?`. Also `Placeholder`, `OnChange`, `Disabled`.

### Table.TableCurrencyInput
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TableCurrencyInput
    @bind-Value="row.SomeAmount"
    Id="some-id"
    Name="some-name"
    CurrencySymbol="$"
    DecimalPlaces="2"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `decimal?`. No `AllowNull` — empty always becomes null. Also `Placeholder`, `OnChange`, `Disabled`.

### Table.TablePercentageInput
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TablePercentageInput
    @bind-Value="row.SomePercentage"
    Id="some-id"
    Name="some-name"
    DecimalPlaces="2"
    AsyncCallBack="async () => await _req.SaveAsync()" />
```
- Value: `decimal?`. Also `AllowNull`, `CallBack`, `Placeholder`, `Disabled`.

### Table.TableCheckBoxInput (both `bool` and `bool?`)
```razor
<!-- nullable bool -->
<FE.Components.Domains.Utlility.Inputs.Table.TableCheckBoxInput
    @bind-NullBool="row.SomeFlag"
    Id="some-id"
    Name="some-name"
    AsyncCallBack="async () => await _req.SaveAsync()" />

<!-- non-nullable bool -->
<FE.Components.Domains.Utlility.Inputs.Table.TableCheckBoxInput
    @bind-NonNullBool="row.SomeFlag"
    Id="some-id"
    Name="some-name" />
```
- Binding mode is picked by which `...Changed` callback is provided. A near-duplicate `TableCheckBoxInput2` exists; use `TableCheckBoxInput` unless told otherwise.

### Table.TableSingleSelectDropDown (no search)
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TableSingleSelectDropDown
    @bind-SelectedOption="row.SomeOption"
    Options="_d.I.Inputs.XxxOptions"
    Id="some-id"
    Name="some-name"
    Placeholder="Select..."
    AsyncCallBack="async (v) => await _req.SaveAsync()" />
```
- Options: `IEnumerable<TableSingleSelectDropDownOption>`. Also `OnChange`, `Disabled`.

### Table.TableSingleDropDownWithSearch
Note the real name — **not** `TableSingleSelectDropDownWithSearch`.
```razor
<FE.Components.Domains.Utlility.Inputs.Table.TableSingleDropDownWithSearch
    @bind-SelectedOption="row.SomeOption"
    Options="_d.I.Inputs.XxxOptions"
    Id="some-id"
    Name="some-name"
    EnableSearch="true"
    Placeholder="Select..."
    AsyncCallBack="async (v) => await _req.SaveAsync()" />
```
- Options: `IEnumerable<TableSingleDropDownWithSearchOption>`. `EnableSearch="false"` shows a "Search options..." button instead. Also `OnChange`, `Disabled`.
