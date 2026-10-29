# Site Brief Template

**Site brief pack · Document 1 of 3** · as at 20 October 2026

| Document | What it is |
|---|---|
| **1. Site Brief Template** (this one) | The form a team member fills in to ask for a website, or a change to one. Includes a filled-in example |
| 2. SEO Factors by Brief Item | What each answer in this brief means for search performance, plus SEO gaps the brief does not cover yet |
| 3. Business Case and Strategy | Why we are building this process, how it works, what has been done so far, and what is still open |

New to this? Read document 3 first, then this one, then document 2.

## What this brief is for

We are automating how websites for our own brands get built. Instead of designing and coding each page by hand, a team member fills in this brief and sends it to Claude (an AI assistant working inside our development project). Claude reads it, asks about anything missing, shows a short build plan for approval, then builds the pages and shows them in a preview.

The brief has to capture everything that would otherwise be decided in meetings: what the site is, how it looks, which pages exist, what goes on each page and in what order, and how any forms behave. Anything left blank becomes a question before building starts.

## How to read the rest of this document

- **Part A** is the blank template, exactly as Claude uses it. Copy it to start a new brief.
- **Part B** is a completed example: the brief for **JJ's Vehicle Finance**, the first site built with this process (a motor-vehicle finance brand with a Home page and a Vehicles page). It is written as if those pages were being requested for the first time, so you can see what a finished brief looks like.

Some words you will meet:

| Term | Meaning |
|---|---|
| Section | One horizontal band of a page (a hero banner, a grid of cards, a calculator...). Pages are built by stacking sections in order |
| Section type | A standard layout from the section library (Hero, Tile grid, Card grid, Numbered steps, Calculator, Reviews, Closing call to action, Text block...). The brief picks types; it does not design layouts from scratch |
| Placeholder | Obviously fake text Claude writes when real copy has not been supplied. It must be replaced before a page goes public |
| Mock / pretend submit | The development environment has no database or email, so forms show their result on screen without saving or sending anything |
| Palette and colour roles | The site's colours, and which colour each part of a page uses. Every site has its own; the build process itself has no built-in look |

> The working copy of this template lives with Claude's build instructions (`.claude/skills/website-component-generator/brief-template.md`). This document is a snapshot for sharing; if the two differ, the working copy is current.

---

# Part A · Blank template
## Site Brief

> **How to use this:** copy this file, fill it in, and send it in chat (paste it or attach it).
> - Replace every `_____` and tick boxes with `[x]`.
> - Delete rows and blocks you don't need.
> - "Placeholder" is always a valid answer for copy. Claude will write clearly fake text.
> - Anything left blank becomes a question before the build starts, so a fuller brief means fewer questions.
> - Parts 1–3 are filled in once per site. Parts 4–6 are filled in for each page you want built or changed.
> - A filled-in example is in `brief-example-jjs-vehicle-finance.md`.

---

### Part 1 · The site

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

### Part 2 · Brand

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

### Part 3 · Navigation and footer

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

### Part 4 · Pages

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

### Part 5 · Sections (one block per page)

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

#### Page: _____

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

### Part 6 · Forms and inputs

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

### Part 7 · Anything else

_____

---

# Part B · Completed example: JJ's Vehicle Finance
## Site Brief · Example: JJ's Vehicle Finance

> A filled-in copy of `brief-template.md`. It describes the Home and Vehicles pages as they exist in Sandox, written as if they were being requested for the first time. Use it to see what a complete brief looks like.

---

### Part 1 · The site

| | |
|---|---|
| Business name (as shown on the site) | JJ's Vehicle Finance |
| What it sells, in one line | Finance for cars, utes, motorbikes, caravans, boats and commercial vehicles |
| Who it's for | People and small businesses buying a vehicle, new or used, private or dealer sale |
| Tone of voice | [x] Bold and punchy |
| Phone number to show | placeholder |
| Email to show | none |
| Where the site lives | [x] Inside Sandox, alongside the existing sites |

**Compliance and things the site must never say**

Finance site: no interest rates, fees, approval times or eligibility claims. All copy is placeholder until approved. Every repayment figure carries "Estimate only. Not a quote or an offer of finance." Reviews are mock and labelled as placeholders; no star ratings or review counts.

---

### Part 2 · Brand

**Palette**

| | |
|---|---|
| Main colour (hero, top bar, panels) | Amber `amber-400` |
| Accent colour (numbers, highlights, form field focus) | Amber: `amber-400` on dark and coloured backgrounds, `amber-600` for small text on white. Site accent class: `.site-jjs-vehicle-finance` |
| Dark colour (text, buttons, footer) | Ink `slate-950` |
| Tint (alternate section background) | `amber-50` |
| Grey (borders, muted text) | Slate: `slate-200` borders, `slate-600` muted text on white, `slate-400` muted text on dark |
| Error colour | Red |
| Success colour | Green (not used yet) |

**Shape and type**

| | |
|---|---|
| Heading style | [x] Heavy and tight |
| Logo | [x] Brand name as text (uppercase, heavy) |
| Button shape | [x] Pill (fully rounded) |
| Card and panel corners | [x] Very rounded |

**Colour roles**

| Role | Background | Text | Border / outline | Notes |
|---|---|---|---|---|
| Page background | White | Dark | | |
| Section background | White | Dark | | |
| Alternate section background | Tint | Dark | | used behind "How it works" |
| Dark band | Dark | White | | used behind the calculator |
| Top bar | Main | Dark | | |
| Hero | Main | Dark | | |
| Footer | Dark | Grey | | footer link column titles: Accent |
| Heading | | Dark | | on a dark background: White |
| Eyebrow label | none | Accent | | in the hero: Dark pill with Accent text |
| Body text | | Dark | | muted text: Grey |
| Link | | Dark | | on hover: underline |
| Primary button | Dark | White | | on hover: slightly lighter Dark. On a Dark band the primary button flips to Main with Dark text |
| Secondary button | none | Dark | Dark | on hover: Dark background, White text |
| Card | White on Main or Tint; light Grey on White | Dark | faint Grey | |
| Number / highlight | | Accent | | step numbers, the repayment figure, ticks |
| Form field | White | Dark | Grey | focus outline uses the Accent colour |
| Validation summary | | Error | | |
| Error box | light Error | Error | light Error | |
| Success message | | | | not used yet |
| Loading message | | Grey | | |
| Spinner | | Accent | | |

No tables or pop-ups on this site yet, so the second role table is left out.

**Reference sites**

| Site address | What to take from it |
|---|---|
| moneyme.com.au | Layout only: sticky coloured top bar, big centred hero headline on a bright colour, product tiles under the headline, statement plus feature columns, reviews, closing "get a quote" band, dark footer with link columns and small print |

---

### Part 3 · Navigation and footer

**Top bar**

| Link label | Goes to (page URL, or page URL + `#section`) |
|---|---|
| Vehicles | /vehicles |

> On the Vehicles page itself the single link is **Home → /home**.

| | |
|---|---|
| Main button label | Get a quote |
| Main button goes to | /home#calculator |
| Show phone in top bar | [x] Yes |

**Footer**

| | |
|---|---|
| One line under the brand name | Finance for cars, utes, motorbikes, caravans and more. |
| Small print | placeholder (must say it is a sandbox page and that figures are examples, not an offer of finance) |

| Column title | Links (label → target), one per line |
|---|---|
| Vehicle finance | Car finance → /home#vehicle-types · Ute finance → /home#vehicle-types · Motorbike finance → /home#vehicle-types · Caravan finance → /home#vehicle-types |
| Tools | Repayment calculator → /home#calculator · How it works → /home#how-it-works |
| Company | Why choose us → /home#why-choose-us · Customer stories → /home#reviews · Contact us → /home#get-started |
| Legal | Privacy policy (placeholder) → /home · Terms of use (placeholder) → /home · Complaints (placeholder) → /home |

---

### Part 4 · Pages

| Page name | URL | Operation | What the page is for | In top bar? |
|---|---|---|---|---|
| Home | /home | [x] Create | Landing page: explain the offer and get people to the calculator | [ ] No (brand name links here) |
| Vehicles | /vehicles | [x] Create | The different types of vehicle finance, one card each | [x] Yes |

**Out of scope for this request**

No other pages. Tile and card buttons go to the calculator on Home until vehicle-specific pages exist.

---

### Part 5 · Sections

#### Page: Home

**Section 1**

| | |
|---|---|
| Type | Hero |
| Section name | Hero |
| Eyebrow label | Motor vehicle finance |
| Heading | Vehicle finance without the runaround |
| Intro line | Cars, utes, motorbikes, caravans and more. Tell us what you are buying and see what the repayments could look like. |
| Buttons (label → target) | Calculate repayments → /home#calculator · How it works → /home#how-it-works |
| Copy | [x] Real (supplied above) |

| Title / name | Description / quote | Extra |
|---|---|---|
| New and used vehicles | | highlight point |
| Private and dealer sales | | highlight point |
| Personal and business use | | highlight point |

**Section 2**

| | |
|---|---|
| Type | Tile grid |
| Section name | VehicleTypes |
| Eyebrow label / Heading / Intro | none (tiles only) |
| Copy | [x] Placeholder |

| Title / name | Description / quote | Extra |
|---|---|---|
| Cars | placeholder | → /home#calculator |
| Utes | placeholder | → /home#calculator |
| Motorbikes | placeholder | → /home#calculator |
| Caravans | placeholder | → /home#calculator |
| Boats | placeholder | → /home#calculator |
| Commercial vehicles | placeholder | → /home#calculator |

**Section 3**

| | |
|---|---|
| Type | Statement and features |
| Section name | WhyChooseUs |
| Eyebrow label | Why choose us |
| Heading | A lender that speaks driver, not banker. |
| Intro line | Vehicle finance is all we do, so the whole process is built around getting you the keys. |
| Copy | [x] Placeholder (feature descriptions) |

| Title / name | Description / quote | Extra |
|---|---|---|
| Quotes without the runaround | placeholder | |
| Terms that fit your budget | placeholder | |
| Real people on the phone | placeholder | |
| Paperwork in plain English | placeholder | |

**Section 4**

| | |
|---|---|
| Type | Calculator |
| Section name | Calculator |
| Eyebrow label | Repayment calculator |
| Heading | See your repayments before you commit. |
| Intro line | Enter the amount, the term and an interest rate to get an estimate in seconds. |
| Copy | [x] Real (supplied above) |

> Fields are in Part 6.

**Section 5**

| | |
|---|---|
| Type | Numbered steps |
| Section name | HowItWorks |
| Eyebrow label | How it works |
| Heading | Three steps to the driver's seat |
| Copy | [x] Placeholder (step descriptions) |

| Title / name | Description / quote | Extra |
|---|---|---|
| Enquire | placeholder | |
| Get assessed | placeholder | |
| Drive away | placeholder | |

**Section 6**

| | |
|---|---|
| Type | Reviews |
| Section name | Testimonials |
| Eyebrow label | Customer stories |
| Heading | Drivers who have been down this road. |
| Copy | [x] Placeholder |

| Title / name | Description / quote | Extra |
|---|---|---|
| Mock customer A | placeholder review about a new car | |
| Mock customer B | placeholder review about a work ute | |
| Mock customer C | placeholder review about a caravan | |

**Section 7**

| | |
|---|---|
| Type | Closing call to action |
| Section name | CallToAction |
| Heading | Ready when you are. |
| Intro line | Run the numbers now and see what your next vehicle could cost each month. |
| Buttons (label → target) | Get a quote → /home#calculator |
| Copy | [x] Real (supplied above) |

| Title / name | Description / quote | Extra |
|---|---|---|
| | Takes a few minutes | point |
| | No obligation | point |
| | Talk to a real person | point |

#### Page: Vehicles

**Section 1**

| | |
|---|---|
| Type | Hero |
| Section name | Hero |
| Eyebrow label | Vehicles |
| Heading | Finance for whatever you drive |
| Intro line | Pick the kind of vehicle you are buying to see how we can help. |
| Buttons | none |
| Copy | [x] Real (supplied above) |

| Title / name | Description / quote | Extra |
|---|---|---|
| New and used vehicles | | highlight point |
| Private and dealer sales | | highlight point |
| Personal and business use | | highlight point |

**Section 2**

| | |
|---|---|
| Type | Card grid |
| Section name | FinanceTypes |
| Eyebrow label | Types of vehicle finance |
| Heading | One lender, every kind of vehicle. |
| Buttons | Each card: Calculate repayments → /home#calculator |
| Copy | [x] Placeholder (descriptions and points) |

| Title / name | Description / quote | Extra |
|---|---|---|
| Car finance | placeholder | 3 placeholder points |
| Ute finance | placeholder | 3 placeholder points |
| Motorbike finance | placeholder | 3 placeholder points |
| Caravan finance | placeholder | 3 placeholder points |
| Boat finance | placeholder | 3 placeholder points |
| Commercial vehicle finance | placeholder | 3 placeholder points |

---

### Part 6 · Forms and inputs

**Section: Home › Calculator**

| Field label | Input type | Starts | Required? | Rules (min / max / format) |
|---|---|---|---|---|
| Loan amount | Money | [x] Blank | [x] Required | More than zero |
| Loan term (years) | Whole number | [x] Blank | [x] Required | 1 to 7 |
| Interest rate (% p.a.) | Percentage | [x] Blank | [x] Required | 0 to 100 |

| | |
|---|---|
| Button label | Calculate |
| What happens when pressed | Check the fields, then work out a standard loan repayment (interest compounding monthly, no fees or balloon) |
| What the result shows | Monthly repayment (large), plus fortnightly, weekly, total interest and total repayable |
| Disclaimer under the result | Estimate only. Not a quote or an offer of finance. Excludes fees and charges, and assumes the rate you enter stays the same for the whole term. |

---

### Part 7 · Anything else

Navigation and footer are per page (each page has its own copy), so later changes to either must be made on every page.
