# Site Brief

> **How to use this:** copy this file, fill it in, and send it in chat (paste it or attach it).
> - Replace every `_____` and tick boxes with `[x]`.
> - Delete rows and blocks you don't need.
> - "Placeholder" is always a valid answer for copy. Claude will write clearly fake text.
> - Anything left blank becomes a question before the build starts, so a fuller brief means fewer questions.
> - Parts 1–3 are filled in once per site. Parts 4–6 are filled in for each page you want built or changed.
> - A filled-in example is in `brief-example-jjs-vehicle-finance.md`.

---

## Part 1 · The site

| | |
|---|---|
| Business name (as shown on the site) | _____ |
| What it sells, in one line | _____ |
| Who it's for | _____ |
| Tone of voice | [ ] Plain and friendly  [ ] Bold and punchy  [ ] Formal and trustworthy  [ ] Other: _____ |
| Phone number to show | _____ (or "placeholder" for `1300 000 000`, or "none") |
| Email to show | _____ (or "none") |
| Where the site lives | [ ] Inside Sandox, alongside the existing sites  [ ] Its own project  [ ] Not decided |

**Compliance and things the site must never say**
> E.g. "Finance site: no interest rates, fees, approval times or eligibility claims. Every repayment figure carries an 'estimate only' disclaimer."

_____

---

## Part 2 · Brand

**Palette**
> Colour name or hex code for each. These names (Main, Accent, Dark, Tint, Grey, Error, Success, plus White) are what the role table below uses.

| | |
|---|---|
| Main colour (hero, top bar, panels) | _____ |
| Accent colour (numbers, highlights, form field focus) | _____ |
| Dark colour (text, buttons, footer) | _____ |
| Tint (alternate section background) | _____ (or "work it out from the main colour") |
| Grey (borders, muted text) | _____ |
| Error colour | _____ |
| Success colour | _____ |

**Shape and type**

| | |
|---|---|
| Heading style | [ ] Heavy and tight  [ ] Light and airy  [ ] Rounded and friendly  [ ] Other: _____ |
| Logo | [ ] Brand name as text  [ ] Logo file attached: _____ |
| Button shape | [ ] Pill (fully rounded)  [ ] Rounded corners  [ ] Square |
| Card and panel corners | [ ] Very rounded  [ ] Slightly rounded  [ ] Square |

**Colour roles**
> Says which palette colour each part of a page uses. For each row, write a palette name (Main, Accent, Dark, Tint, Grey, Error, Success, White), or "none".
> - **Leave a cell blank** and Claude proposes it in the plan for you to approve.
> - **Leave the whole table blank** and Claude proposes all of it.
> - The role names match the placeholders in the skill's templates, so every answer here maps straight onto the code.

| Role | Background | Text | Border / outline | Notes |
|---|---|---|---|---|
| Page background | _____ | _____ | | |
| Section background | _____ | _____ | | the usual section |
| Alternate section background | _____ | _____ | | every other section, if you want them to alternate |
| Dark band | _____ | _____ | | a strong contrasting section, e.g. behind a calculator |
| Top bar | _____ | _____ | | |
| Hero | _____ | _____ | | |
| Footer | _____ | _____ | | footer link column titles: _____ |
| Heading | | _____ | | on a dark background: _____ |
| Eyebrow label | _____ | _____ | | the small label above a heading |
| Body text | | _____ | | muted text: _____ |
| Link | | _____ | | on hover: _____ |
| Primary button | _____ | _____ | _____ | on hover: _____ |
| Secondary button | _____ | _____ | _____ | on hover: _____ |
| Card | _____ | _____ | _____ | |
| Number / highlight | | _____ | | step numbers, big figures, ticks |
| Form field | _____ | _____ | _____ | focus outline uses the Accent colour |
| Validation summary | _____ | _____ | _____ | the "Please check: …" line under a form |
| Error box | _____ | _____ | _____ | when something fails to load |
| Success message | _____ | _____ | _____ | |
| Loading message | | _____ | | |
| Spinner | | _____ | | |

Only if your pages have tables or pop-ups:

| Role | Background | Text | Border / outline | Notes |
|---|---|---|---|---|
| Table header cell | _____ | _____ | _____ | |
| Table cell, primary | | _____ | _____ | the first column |
| Table cell, secondary | | _____ | _____ | the other columns |
| Modal backdrop | _____ | | | the dimmed page behind a pop-up |
| Modal panel | _____ | _____ | _____ | |
| Modal header | _____ | _____ | | |
| Modal title | | _____ | | |
| Modal close button | | _____ | | on hover: _____ |

**Reference sites**
> Claude takes layout and structure only, never another site's branding, colours or wording.

| Site address | What to take from it |
|---|---|
| _____ | _____ |
| _____ | _____ |

---

## Part 3 · Navigation and footer

These are copied onto every page.

**Top bar**

| Link label | Goes to (page URL, or page URL + `#section`) |
|---|---|
| _____ | _____ |
| _____ | _____ |

| | |
|---|---|
| Main button label | _____ (e.g. "Get a quote") |
| Main button goes to | _____ |
| Show phone in top bar | [ ] Yes  [ ] No |

**Footer**

| | |
|---|---|
| One line under the brand name | _____ |
| Small print | _____ (or "placeholder") |

| Column title | Links (label → target), one per line |
|---|---|
| _____ | _____ → _____ |
| _____ | _____ → _____ |

---

## Part 4 · Pages

One row per page in this request.

| Page name | URL | Operation | What the page is for | In top bar? |
|---|---|---|---|---|
| _____ | /_____ | [ ] Create  [ ] Edit  [ ] Remove | _____ | [ ] Yes  [ ] No |
| _____ | /_____ | [ ] Create  [ ] Edit  [ ] Remove | _____ | [ ] Yes  [ ] No |

> **Edit:** in Part 5, list only the sections that change, and say what changes.
> **Remove:** no Part 5 needed. Claude will list every file to be deleted and confirm with you first.

**Out of scope for this request**
> List anything Claude should **not** build or change this time, even if the pages above seem to need it. Only the pages in the table above get built. This box is for the grey areas around them.
>
> Examples:
> - "The top bar links to /about and /contact, but don't create those pages yet." The links still appear; until the pages exist they show the site's "Page not found" message, unless you say where they should point for now (e.g. "point them at /home").
> - "Don't change the Home page, even though it links to the new page."
> - "No contact form yet; the 'Contact us' button just goes to /home."
>
> If you leave this blank, Claude builds only the pages listed above and asks before touching anything else.

_____

---

## Part 5 · Sections (one block per page)

Pick each section's **type** from the section library (`sections.md`):
- Hero
- Tile grid
- Statement and features
- Card grid
- Numbered steps
- Calculator
- Reviews
- Closing call to action
- Text block
- New type (describe it)

List sections in the order they appear, top to bottom.

### Page: _____

**Section 1**

| | |
|---|---|
| Type | _____ |
| Section name (one or two words, becomes the folder name) | _____ (e.g. `Hero`, `FinanceTypes`) |
| Eyebrow label (small text above the heading) | _____ |
| Heading | _____ |
| Intro line | _____ |
| Buttons (label → target) | _____ → _____ |
| Copy | [ ] Real (supplied below)  [ ] Placeholder |

Items (tiles, cards, features, steps, reviews or points):

| Title / name | Description / quote | Extra (points, link target) |
|---|---|---|
| _____ | _____ | _____ |
| _____ | _____ | _____ |

**Section 2**

> Copy the Section 1 block for each further section.

---

## Part 6 · Forms and inputs

Only needed if a section has fields (e.g. a Calculator). Sandox has no backend, so any submit is pretended.

**Section: _____**

| Field label | Input type | Starts | Required? | Rules (min / max / format) |
|---|---|---|---|---|
| _____ | _____ | [ ] Blank  [ ] Pre-filled: _____ | [ ] Required  [ ] Optional | _____ |
| _____ | _____ | [ ] Blank  [ ] Pre-filled: _____ | [ ] Required  [ ] Optional | _____ |

> **Input types:** Text, Long text, Whole number, Money, Percentage, Date, Date and time, Time, Mobile number, Yes/No, Toggle, Dropdown (list the options), Dropdown with search, Multi-select (list the options).

| | |
|---|---|
| Button label | _____ |
| What happens when pressed | _____ (e.g. "work out the monthly repayment", "show a thank-you message") |
| What the result shows | _____ |
| Disclaimer under the result | _____ |

---

## Part 7 · Anything else

_____
