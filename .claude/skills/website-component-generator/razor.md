# Razor File Reference (Sandox)

## Critical Rules
- **Never** add `@inherits` to a razor file
- **Never** add `@namespace` to a razor file (the code-behind's `namespace` line decides it)
- Tailwind classes only. This file contains **no styling**: wherever a template needs colours, fonts, sizes, borders, radius or shadows it shows a `{style: <role>}` placeholder. Replace each with the site's own classes for that role (see "Styling" in `SKILL.md`). Never output a placeholder literally. Layout and behaviour classes (`flex`, `gap-3`, `sticky top-0`, `fixed inset-0`, `animate-spin`, icon sizes) are kept as written.
- New Tailwind classes only appear after the CSS is rebuilt (`npm run build:css` or `npm run watch:css`). Do not build class names dynamically (`bg-@color-500` never works); write the full class.

---

## Standard Three-State Gate Pattern

Every component's root element uses `_d.I.ClassName` as the id.
The loading check ALWAYS combines `InitialisationCompleted` and `RequestIsLoading` in the same `@if`:

```razor
<div id="@_d.I.ClassName">
    @if (_d.I.InitialisationCompleted == false || _d.RC_Internal.Initialise_HD_.ResponseData.RequestStatus.RequestIsLoading)
    {
        <div class="flex items-center gap-3 {style: loading message}">
            <svg class="animate-spin w-4 h-4 {style: spinner}" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
            </svg>
            Loading...
        </div>
    }
    else if (_d.RC_Internal.Initialise_HD_.ResponseData.RequestStatus.RequestHasFailed)
    {
        <div class="flex items-start gap-3 {style: error box}">
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

The root element may also carry the section's own background or layout classes (`{style: section background}`).

---

## Hosting on a page

The page component (`<Area>_P_C`) is itself routable and hosts its three groups. There is no separate thin page file:

```razor
@page "/ticket"
@using FE.Components.Domains.Pages.Ticket.Navigation.A
@using FE.Components.Domains.Pages.Ticket.Sections.A
@using FE.Components.Domains.Pages.Ticket.Footer.A

<PageTitle>Ticket</PageTitle>

<div id="@_d.I.ClassName" class="site-<site-name> {style: page background}">
    <!-- three-state gate as above, with this in the completed branch: -->
    <Ticket_Navigation_C />
    <Ticket_Sections_C />
    <Ticket_Footer_C />
</div>
```

- The `@page` route must be unique, and only the `_P_C` component has one.
- `site-<site-name>` on the page root is the site's accent class from `wwwroot/input.css` (see "Styling" in `SKILL.md`). It makes every shared input, dropdown and modal on the page use that site's accent.
- `_Imports.razor` already has `@using FE` and `@using FE.Components`. Add a `@using` for each child's namespace, or write the fully-qualified tag.
- The layout (`Components/Layout/MainLayout.razor`) is owned by the user. Do not edit it unless asked.

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
                            <th scope="col" class="sticky top-0 z-10 py-3.5 pr-3 pl-4 text-left sm:pl-6 lg:pl-8 {style: table header cell}">Column 1</th>
                            <th scope="col" class="sticky top-0 z-10 px-3 py-3.5 text-left {style: table header cell}">Column 2</th>
                            <th scope="col" class="sticky top-0 z-10 py-3.5 pr-4 pl-3 text-left sm:pr-6 lg:pr-8 {style: table header cell}">Column 3</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var row in _d.RC_Internal.Initialise_HD_.ResponseData.Rows)
                        {
                            <tr>
                                <td class="py-4 pr-3 pl-4 whitespace-nowrap sm:pl-6 lg:pl-8 {style: table cell, primary}">@row.Value1</td>
                                <td class="px-3 py-4 {style: table cell, secondary}">@row.Value2</td>
                                <td class="py-4 pr-4 pl-3 whitespace-nowrap sm:pr-6 lg:pr-8 {style: table cell, secondary}">@row.Value3</td>
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

### Razor gotcha: no string literals inside a multi-line `@onclick` lambda

A double-quoted C# string inside an `@onclick="async () => { ... }"` block breaks the Razor compiler. `@onclick="async () => { _state.SetFilter("all"); }"` fails with `CS7036` (the argument silently disappears) and `@onclick="async () => { await _req.SaveAsync(id, "close"); }"` fails with `CS1525 Invalid expression term ')'`.

Do not pass string literals from the razor. Instead:
- give the inner class a method that needs no argument (`_state.ShowAllNatures()`, `_req.CloseTicketAsync(id)`, `_req.LogAttemptAsync(id)`) and let that method use the literal in the `.cs` file, or
- pass a variable/loop value (`_state.SetNatureFilter(nature.Key)` is fine).

String literals inside a plain `class="@_switch.FilterChip("all")"` attribute or in element content (`@_act.CountNature("all")`) do compile, but prefer a variable in an attribute if the compiler complains.

### Razor gotcha: in-page anchors must include the page route

`Components/App.razor` has `<base href="/">`, so a bare `<a href="#shop">` resolves to `/#shop` and **navigates to the site root instead of scrolling**. Always write the page's route in front of the fragment:

```razor
<a href="/test#shop">Shop</a>      @* correct: same-page navigation, scrolls to id="shop" *@
<a href="#shop">Shop</a>           @* wrong: leaves the page for "/" *@
```

Use the component's own `@page` route. A link that also needs to change state can combine both: `<a href="/test#shop" @onclick="async () => { _state.ShowGifts(); await _d.StateUpdate(); }">`.

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
<div class="@_switch.GetRowCss()">
```

`_switch` methods return complete class strings. Never assemble a class name from parts that Tailwind cannot see as a whole. The strings it returns are the site's own classes.

---

## Modals

Prefer the existing components rather than hand-writing the markup:

```razor
<FE.Components.Domains.Utlility.Modal.GenericModal_C Title="Example" Width="480px" OnClose="() => _d.I.ShowModal = false">
    <p class="p-4 {style: body text}">Content</p>
</FE.Components.Domains.Utlility.Modal.GenericModal_C>
```

`GenericModal_C` parameters: `Title`, `Width`, `Height`, `ShowCloseButton` (true), `FullScreen` (false), `OnClose`, `ChildContent`. Its own appearance is defined inside that shared component, not here.

If a fully custom modal shell is needed, use this:

```razor
<div class="fixed inset-0 z-100 flex items-center justify-center {style: modal backdrop}"
     @onclick="async () => await _act.Close()">
    <div class="z-50 flex flex-col {style: modal panel}"
         style="width: @(_d.I.Width); height: @(_d.I.Height)"
         @onclick:stopPropagation="true">
        <div class="px-4 py-3 flex items-center justify-between {style: modal header}">
            <h3 class="{style: modal title}">@_d.I.Title</h3>
            @if (_d.I.ShowCloseButton)
            {
                <button @onclick="async () => await _act.Close()"
                        class="{style: modal close button}"
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
         class="fixed overflow-hidden z-50 {style: modal panel}"
         style="width: @(Width != null ? Width+"px" : ""); min-height: @(Height)px; left: @(InitialX)px; top: @(InitialY)px;"
         @ref="draggableElement">
        <div id="draggable-header-@componentId"
             class="px-4 py-3 cursor-move select-none flex items-center justify-between {style: modal header}">
            <h3 class="{style: modal title}">@Title</h3>
            @if (ShowCloseButton)
            {
                <button @onclick="Close"
                        class="{style: modal close button}"
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

A child is referenced by tag (with a `@using` for its namespace) or by full namespace path. A child that needs data from its parent takes `[Parameter]`s; one that shares page state uses the injected page `_S`:

```razor
<Test_Sections_Hero_C />

<Test_Sections_ProductGrid_Card_C @key="product.Id"
                                  Product="product"
                                  IsAdding="@_act.IsAddingToBasket()"
                                  OnAddToBasket="async (int productId) => {
                                      await _req.AddToBasketAsync(productId);
                                      await _d.StateUpdate();
                                  }" />
```

---

## Loading Spinner (standalone)

Use this whenever showing a loading state inside already-rendered content:

```razor
<div class="flex items-center gap-3 {style: loading message}">
    <svg class="animate-spin w-4 h-4 {style: spinner}" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
    </svg>
    Loading...
</div>
```
