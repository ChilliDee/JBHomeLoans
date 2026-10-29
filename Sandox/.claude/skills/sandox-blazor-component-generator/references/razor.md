# Razor File Reference (Sandox)

## Critical Rules
- **Never** add `@inherits` to a razor file
- **Never** add `@namespace` to a razor file — the namespace comes from the folder path and must match the code-behind
- Page components (routable) start with `@page "/route"`. Non-page components have no directive at all.
- Sandox's `_Imports.razor` only imports `FE` and `FE.Components`, so reference input components and child components by their full namespace path.
- Inside an existing C# block (`else if (...) { ... }`) write `if` / `foreach` **without** the `@` — `@if` there is a Razor compile error (RZ1010). Use `@if` / `@foreach` only at markup level.
- Tailwind is prebuilt into `wwwroot/output.css`. New classes only take effect after `npm run build:css` (or `watch:css`).

Page example (from `Demo_P_List_C.razor`):

```razor
@page "/demo"

<div id="@_d.I.ClassName" class="bg-white rounded-lg border border-gray-200 overflow-hidden">
  ...
</div>
```

---

## Standard Three-State Gate Pattern

Every component's root element uses `_d.I.ClassName` as the id.
The loading check ALWAYS combines `InitialisationCompleted` and `RequestIsLoading` in the same `@if`:

```razor
<div id="@_d.I.ClassName">
    @if (_d.I.InitialisationCompleted == false || _d.RC_Internal.Initialise_HD_.ResponseData.RequestStatus.RequestIsLoading)
    {
        <div class="flex items-center gap-3 p-6 text-sm text-gray-500">
            <svg class="animate-spin w-4 h-4 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
            </svg>
            Loading...
        </div>
    }
    else if (_d.RC_Internal.Initialise_HD_.ResponseData.RequestStatus.RequestHasFailed)
    {
        <div class="mx-4 my-3 flex items-start gap-3 p-4 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">
            <svg class="w-4 h-4 mt-0.5 shrink-0" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z" />
            </svg>
            @_d.RC_Internal.Initialise_HD_.ResponseData.ErrorMessage
        </div>
    }
    else if (_d.RC_Internal.Initialise_HD_.ResponseData.RequestStatus.RequestHasCompleted)
    {
        <!-- your content here -->
    }
</div>
```

---

## Standard Table Structure

```razor
<div class="px-4 sm:px-6 lg:px-8">
    <div class="flow-root">
        <div class="-mx-4 -my-2 sm:-mx-6 lg:-mx-8">
            <div class="inline-block min-w-full py-2 align-middle">
                <table class="min-w-full border-separate border-spacing-0">
                    <thead>
                        <tr>
                            <th scope="col" class="sticky top-0 z-10 border-b border-gray-300 bg-white/75 py-3.5 pr-3 pl-4 text-left text-sm font-semibold text-gray-900 backdrop-blur-sm backdrop-filter sm:pl-6 lg:pl-8">Column 1</th>
                            <th scope="col" class="sticky top-0 z-10 border-b border-gray-300 bg-white/75 px-3 py-3.5 text-left text-sm font-semibold text-gray-900 backdrop-blur-sm backdrop-filter">Column 2</th>
                            <th scope="col" class="sticky top-0 z-10 border-b border-gray-300 bg-white/75 py-3.5 pr-4 pl-3 text-left text-sm font-semibold text-gray-900 backdrop-blur-sm backdrop-filter sm:pr-6 lg:pr-8">Column 3</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var row in _d.RC_Internal.Initialise_HD_.ResponseData.Rows)
                        {
                            <tr>
                                <td class="border-b border-gray-200 py-4 pr-3 pl-4 text-sm whitespace-nowrap text-gray-900 sm:pl-6 lg:pl-8">@row.Value1</td>
                                <td class="border-b border-gray-200 px-3 py-4 text-sm text-gray-500">@row.Value2</td>
                                <td class="border-b border-gray-200 py-4 pr-4 pl-3 text-sm whitespace-nowrap text-gray-500 sm:pr-6 lg:pr-8">@row.Value3</td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>
```

---

## onclick Pattern

Always use inline async lambdas. Call methods from inner classes in sequence:

```razor
@onclick="async() => {
    await _req.SomeRequestAsync();
    _state.SomeStateMethod();
    _act.SomeCalculation();
    await _d.StateUpdate();
}"
```

Rules:
- `_req.*` — RC execute calls only
- `_state.*` — writing to ObservableProperties
- `_act.*` — general logic, boolean visibility checks
- `_switch.*` — CSS strings only, used in `class="@_switch.GetSomeCss()"`
- `await _d.StateUpdate()` — triggers re-render, place at start or end as needed

---

## Visibility Pattern

```razor
@if (_act.ShowSomething())
{
    <!-- content -->
}
```

---

## CSS Switch Pattern

```razor
<div class="@_switch.GetRowColor()">
```

---

## Generic Modal

```razor
<div class="fixed inset-0 bg-black/40 z-100 flex items-center justify-center"
     @onclick="async () => await _act.Close()">
    <div class="bg-white rounded-lg shadow-2xl border border-gray-300 z-50 flex flex-col"
         style="width: @(_d.I.Width); height: @(_d.I.Height)"
         @onclick:stopPropagation="true">
        <div class="bg-blue-600 px-4 py-3 flex items-center justify-between">
            <h3 class="text-white font-semibold text-base">@_d.I.Title</h3>
            @if (_d.I.ShowCloseButton)
            {
                <button @onclick="async () => await _act.Close()"
                        class="text-white hover:text-gray-200 transition-colors"
                        type="button">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                </button>
            }
        </div>
        <div class="p-4 flex-1 min-h-0 flex flex-col" id="genericModalBody">
            @_d.I.ChildContent
        </div>
    </div>
</div>
```

---

## Draggable Modal

```razor
@if (isVisible)
{
    <div id="draggable-@componentId"
         class="fixed bg-white rounded-lg shadow-2xl border border-gray-300 overflow-hidden z-50"
         style="width: @(Width != null ? Width+"px" : ""); min-height: @(Height)px; left: @(InitialX)px; top: @(InitialY)px;"
         @ref="draggableElement">
        <div id="draggable-header-@componentId"
             class="bg-blue-600 px-4 py-3 cursor-move select-none flex items-center justify-between">
            <h3 class="text-white font-semibold text-base">@Title</h3>
            @if (ShowCloseButton)
            {
                <button @onclick="Close"
                        class="text-white hover:text-gray-200 transition-colors"
                        type="button">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                </button>
            }
        </div>
        <div class="p-4 pt-0 overflow-auto" style="max-height: calc(100vh - 100px);">
            @ChildContent
        </div>
    </div>
}
```

---

## Child Component Embedding

Child components are referenced by full namespace path (`FE` + folder path + component name), no parameters (they use `_S` services):

```razor
<FE.Components.Domains.Pages.Demo.A.Demo_P_List_C />
```

---

## Loading Spinner (standalone)

Use this whenever showing a loading state inside already-rendered content:

```razor
<div class="flex items-center gap-3 p-6 text-sm text-gray-500">
    <svg class="animate-spin w-4 h-4 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
    </svg>
    Loading...
</div>
```
