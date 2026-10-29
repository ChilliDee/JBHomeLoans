# Section Library (Sandox website pages)

The standard section types a website page is built from. A site brief picks sections from this list by **type name**; each type maps to a built component that is the reference for its **structure**: folder, data shape, the parts it contains and how they are laid out.

This library contains **no styling**. The reference components belong to one site, so copy their structure and markup outline, not their colours, fonts, sizes, radius or shadows. The look of every section comes from the site's own brief (Part 2) and its existing pages; see "Styling" in `SKILL.md`. "Layout" below describes arrangement only.

Every section is a component in its own folder under the page's `Sections/` group (`Pages/<Area>/Sections/<SectionName>/A/<Area>_Sections_<SectionName>_C`). Content always comes from the section's `Initialise_HD` mock data. Headings and one-line intros may sit in the razor.

Navigation and Footer are not sections: every page has one of each (see the last two entries).

---

## Hero

The opening band of a page: eyebrow label, headline, intro line, up to two buttons, and a row of short highlight points.

| | |
|---|---|
| Reference | `Pages/Home/Sections/Hero/A/Home_Sections_Hero_C` (with buttons), `Pages/Vehicles/Sections/Hero/A/Vehicles_Sections_Hero_C` (without buttons) |
| Brief fills in | Eyebrow label, headline, intro line, button 1 (label + target), button 2 (label + target), 0–4 highlight points |
| Data | `List<MockHighlight>` (`Id`, `Text`) |
| Layout | Full-width band, content centred: eyebrow, headline, intro, buttons in a row, highlight points in a row underneath |
| Rules | One per page, always first. If a **Tile grid** follows it, the hero needs extra bottom padding at least as deep as the tiles' overlap |

## Tile grid

A grid of clickable tiles (title, one line, arrow) that overlaps the bottom of the hero.

| | |
|---|---|
| Reference | `Pages/Home/Sections/VehicleTypes/A/Home_Sections_VehicleTypes_C` |
| Brief fills in | 3–9 tiles: title, one line, link target |
| Data | `List<Mock...>` (`Id`, `Title`, `Description`); add `Href` if tiles go to different places |
| Layout | Grid of 1 / 2 / 3 columns (phone / tablet / desktop). Pulled up with a negative top margin so it sits over the end of the hero, and placed above it (`relative z-10`) |
| Rules | Only directly after a Hero |

## Statement and features

A large statement heading with a short intro, then a row of feature columns.

| | |
|---|---|
| Reference | `Pages/Home/Sections/WhyChooseUs/A/Home_Sections_WhyChooseUs_C` |
| Brief fills in | Eyebrow label, statement heading, intro line, 3–4 features (title + one or two sentences) |
| Data | `List<Mock...>` (`Id`, `Title`, `Description`) |
| Layout | Left-aligned heading block, then 1 / 2 / 4 feature columns, each with a divider above its title |

## Card grid

Numbered cards, each with a title, description, bullet points and a button.

| | |
|---|---|
| Reference | `Pages/Vehicles/Sections/FinanceTypes/A/Vehicles_Sections_FinanceTypes_C` |
| Brief fills in | Eyebrow label, heading, 2–9 cards: title, description, 0–5 points, button label + target |
| Data | `List<Mock...>` (`Id`, `Title`, `Description`, `List<string> Points`) |
| Layout | Heading block, then a grid of 1 / 2 / 3 columns. Each card stacks: number, title, description, divider, points, button at the bottom (cards in a row line up) |

## Numbered steps

A short process shown as numbered cards.

| | |
|---|---|
| Reference | `Pages/Home/Sections/HowItWorks/A/Home_Sections_HowItWorks_C` |
| Brief fills in | Eyebrow label, heading, 3–4 steps: title + one sentence |
| Data | `List<Mock...>` (`Id`, `Title`, `Description`) |
| Layout | Centred heading block, then 1 / 3 columns of cards, each with a large step number (`01`, `02`...), title and sentence |

## Calculator

A form plus a result panel. The only section type with inputs so far.

| | |
|---|---|
| Reference | `Pages/Home/Sections/Calculator/A/Home_Sections_Calculator_C` |
| Brief fills in | Eyebrow label, heading, intro line, the **input table** (see the brief), what the result shows, the disclaimer line |
| Data | `Initialise_HD` returns option lists and limits; a second RC sub-class (`Calculate`) does the sum and returns the results |
| Layout | Two columns on desktop, stacked on phone: heading, intro and result on one side, the form on the other. The result has three states: empty, calculated, failed |
| Rules | Inputs follow `inputs.md`. Every input needs pre-filled vs blank and required vs optional answered. A calculation about money always carries an "estimate only" style disclaimer |

## Reviews

Customer quote cards.

| | |
|---|---|
| Reference | `Pages/Home/Sections/Testimonials/A/Home_Sections_Testimonials_C` |
| Brief fills in | Eyebrow label, heading, 2–6 reviews: name + quote |
| Data | `List<Mock...>` (`Id`, `Title` = name, `Description` = quote) |
| Layout | Heading block, then 1 / 3 columns of cards: quote mark, quote, divider, name (names line up at the bottom) |
| Rules | Unless the brief supplies real, approved reviews, names are `Mock customer A/B/C` and the quote says it is a placeholder. Never invent star ratings, totals or review counts |

## Closing call to action

A panel near the end of the page with a heading, a line, one button and short reassurance points.

| | |
|---|---|
| Reference | `Pages/Home/Sections/CallToAction/A/Home_Sections_CallToAction_C` |
| Brief fills in | Heading, one line, button label + target, 0–4 short points |
| Data | `List<Mock...>` (`Id`, `Description` = the point) |
| Layout | A single panel inset within the page width, content centred: heading, line, button, points in a row |
| Rules | At most one per page, usually last |

## Text block

A plain block of headed text for content pages (about, policy, explainer).

| | |
|---|---|
| Reference | **Not built yet.** The first one built becomes the reference |
| Brief fills in | Heading, paragraphs (or "placeholder"), optional sub-headings |
| Data | Heading plus `List<Mock...>` of paragraphs or sub-sections |
| Layout | A single narrow, readable column |

---

## Navigation (every page)

| | |
|---|---|
| Reference | `Pages/Home/Navigation/A/Home_Navigation_C` |
| Brief fills in | Links (label + target), phone (or none), main button (label + target) |
| Data | Brand name, phone, `List<MockNavLink>` (`Id`, `Label`, `Href`) |
| Layout | Sticky bar: brand name left, links centre, phone and main button right. Below the large breakpoint the links collapse behind a menu button that opens a stacked list |

## Footer (every page)

| | |
|---|---|
| Reference | `Pages/Home/Footer/A/Home_Footer_C` |
| Brief fills in | One-line description, link columns (title + links), small print |
| Data | Brand name, small print, `List<MockFooterColumn>` (`Title`, `List<MockFooterLink>`) |
| Layout | Brand name and description on one side, up to four link columns on the other; small print and copyright underneath, across the full width |

---

## Adding a new section type

If a brief asks for something none of these types cover, build it as a new section, then add it to this file with its reference component, what the brief must fill in, its data shape, layout and rules. Keep the entry free of styling.
