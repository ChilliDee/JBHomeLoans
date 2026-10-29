# Site Brief · Example: JJ's Vehicle Finance

> A filled-in copy of `brief-template.md`. It describes the Home and Vehicles pages as they exist in Sandox, written as if they were being requested for the first time. Use it to see what a complete brief looks like.

---

## Part 1 · The site

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

## Part 2 · Brand

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

## Part 3 · Navigation and footer

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

## Part 4 · Pages

| Page name | URL | Operation | What the page is for | In top bar? |
|---|---|---|---|---|
| Home | /home | [x] Create | Landing page: explain the offer and get people to the calculator | [ ] No (brand name links here) |
| Vehicles | /vehicles | [x] Create | The different types of vehicle finance, one card each | [x] Yes |

**Out of scope for this request**

No other pages. Tile and card buttons go to the calculator on Home until vehicle-specific pages exist.

---

## Part 5 · Sections

### Page: Home

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

### Page: Vehicles

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

## Part 6 · Forms and inputs

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

## Part 7 · Anything else

Navigation and footer are per page (each page has its own copy), so later changes to either must be made on every page.
