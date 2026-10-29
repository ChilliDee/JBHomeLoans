# SEO Factors by Brief Item

**Site brief pack · Document 2 of 3** · as at 20 October 2026

| Document | What it is |
|---|---|
| 1. Site Brief Template | The form a team member fills in to ask for a website, or a change to one |
| **2. SEO Factors by Brief Item** (this one) | What each answer in the brief means for search performance |
| 3. Business Case and Strategy | Why we are building this process, how it works, what has been done so far |

## How to use this document

Every answer in the site brief (document 1) shapes the pages Claude builds, and most of them affect how well those pages can rank. This document goes through the brief **item by item, in the brief's own order**, and lists the SEO factors each one touches.

It is a working list for the team to turn into brief improvements, not a finished SEO specification. It has four parts:

1. **SEO factors per brief item** (Parts 1 to 7 of the brief).
2. **SEO information the brief does not ask for yet.** These are candidate new fields.
3. **SEO of the build itself.** These are issues in how the pages are produced, which no brief answer can fix. Some are serious and are true of every page built so far.
4. **Suggested priorities.**

Background for readers new to the project: the sites are built with **Blazor** (a Microsoft web framework). Pages are made of reusable components, and each page is a stack of **sections** chosen from a fixed **section library**. The current development environment ("Sandox") uses made-up placeholder content and has no database. Document 3 explains all of this.

**What "Status today" means:** this column describes the first site built with the process, *JJ's Vehicle Finance* (a Home page and a Vehicles page). Items marked **(checked)** were verified against the running pages on 20 October 2026; the rest are design observations.

---

## 1. SEO factors per brief item

### Brief Part 1 · The site

| Brief item | SEO factors to consider | Status today |
|---|---|---|
| **Business name** | Brand/entity consistency: the same name, spelt the same way, in the logo text, page titles, footer, contact details and structured data (`Organization` schema). Branded search depends on Google connecting the name to the site. Decide the title-tag brand suffix format once (e.g. `Page topic · Brand`) | Name is consistent across pages. No structured data |
| **What it sells, in one line** | This is the site's core topic. It should map to a keyword theme and to the Home page's primary target. Each product or service in the line is a candidate for its own landing page (topic clusters) rather than being crammed onto one page | Six vehicle types are listed on Home and again on Vehicles, but none has its own page yet |
| **Who it's for** | Search intent and audience: who searches, what they type, at which stage (researching, comparing, ready to apply). Location targeting: an Australian audience should be reflected in language (`en-AU`), spelling, currency, phone format, and any state or city focus. Personal vs business intent often needs separate pages | The pages declare `lang="en"`, not `en-AU` (checked) |
| **Tone of voice** | Readability and helpfulness (Google's helpful-content guidance), and E-E-A-T (experience, expertise, authoritativeness, trust). Punchy marketing copy still needs substance: specific, factual, written for people. Tone should not override clarity in headings, which carry ranking weight | Copy is placeholder |
| **Phone number to show** | NAP consistency (Name, Address, Phone) across the site, Google Business Profile and directories. Should be a click-to-call `tel:` link on mobile (a usability and conversion signal). Can feed `LocalBusiness` / `Organization` schema | Shown as plain text, not a `tel:` link; number is a placeholder |
| **Email to show** | Trust signal and contact-page content. Mark it up rather than as an image. Consider spam protection | Not shown |
| **Where the site lives** | Domain strategy: own domain vs subdomain vs subfolder of another site. Each has different authority and migration implications. If a site replaces an existing one, redirect mapping and URL preservation become critical | Undecided |
| **Compliance and things the site must never say** | Finance is a **YMYL** topic ("Your Money or Your Life"): Google applies higher quality and trust standards. Required trust content: who the business is, licensing or credit-licence disclosures, complaints process, privacy policy, accurate and current claims. Disclaimers must be crawlable text, not images. Avoid unverifiable superlatives ("best rates", "instant approval"), which damage trust and can breach advertising rules | Disclaimers exist on the calculator and in footer small print; the legal pages they point to do not exist yet |

### Brief Part 2 · Brand

| Brief item | SEO factors to consider | Status today |
|---|---|---|
| **Palette** (Main, Accent, Dark, Tint, Grey, Error, Success) | Not a direct ranking factor. It matters through **accessibility and usability**: text and background combinations need sufficient contrast (WCAG AA: 4.5:1 for body text, 3:1 for large text). Poor contrast hurts engagement and accessibility audits | Not audited |
| **Heading style** | Visual style must stay separate from **semantic heading levels** (`h1`, `h2`, `h3`). Something that looks like a heading should be a heading tag, and heading tags should never be chosen for their size. One `h1` per page | One `h1` per page and `h2` per section on built pages (checked in code) |
| **Logo** | A text logo is crawlable as text. An image logo needs a descriptive `alt`, sensible file size, and can be referenced in `Organization` schema. The logo link should go to the canonical home URL | Text logo linking to `/home` |
| **Button shape / Card and panel corners** | No SEO impact beyond usability: tap targets must be large enough and spaced on mobile | Buttons are large |
| **Colour roles table** | Contrast per role, especially muted text, links and text on Main or Tint. Links in body copy must be distinguishable from surrounding text, not by colour alone. Focus outlines must be visible (accessibility) | Accent focus outlines in place; muted-grey contrast not measured |
| **Reference sites** | Copy **structure only**, never wording. Copied text is duplicate content and a legal risk. Use reference sites for **competitor analysis**: which of their pages rank, for which terms, with which content depth. That should inform which pages and sections the brief asks for | The JJ's brief takes layout only from moneyme.com.au |

### Brief Part 3 · Navigation and footer

| Brief item | SEO factors to consider | Status today |
|---|---|---|
| **Top bar links** | Navigation is the strongest internal-linking signal: it tells search engines which pages matter most and keeps important pages within one click of every page (crawl depth). Use plain crawlable `<a href>` links with descriptive anchor text ("Car finance", not "Click here"). Links to fragments of the same page (`/home#calculator`) do not create new crawlable pages. The mobile menu must contain the same links in the HTML | One link (Vehicles → /vehicles). Earlier same-page jump links were removed |
| **Main button (label + target)** | Conversion, not ranking, but it drives engagement signals. If it targets a page section, consider whether that tool deserves its own URL so it can rank and be linked to | "Get a quote" jumps to the calculator section on Home |
| **Show phone in top bar** | Same as Part 1 phone: `tel:` link, consistent format | Plain text |
| **Footer one-liner** | Sitewide boilerplate. Keep it short and natural. Repeated keyword-stuffed footer text is a spam pattern | Fine |
| **Small print** | Sitewide repeated text adds no unique value per page. Keep legal detail on dedicated legal pages and link to them; keep footer small print brief. Required disclosures (e.g. licence numbers) still belong in the footer for trust | Placeholder text |
| **Footer columns and links** | Sitewide internal links: good for crawl paths to secondary pages (legal, about, contact). Every link should point to a real page. **Links to pages that do not exist produce 404 errors**, and placeholder links that all point at the home page waste link signals | Several footer links point at `/home` or `/home#…` as placeholders |
| **Navigation and footer copied onto each page** | Each page has its own copy of the nav and footer. Inconsistencies between copies (a link added on one page only) create uneven internal linking and broken paths | Two pages, two copies, currently identical apart from the nav link |

### Brief Part 4 · Pages

| Brief item | SEO factors to consider | Status today |
|---|---|---|
| **Page name** | Becomes the page's title and usually its topic. Each page needs a **unique title tag** (about 50–60 characters, primary term first, brand last) and a **unique meta description**. The brief does not ask for either yet (see part 2 of this document) | Titles are "JJ's Vehicle Finance" and "Vehicles · JJ's Vehicle Finance" (checked); no meta descriptions (checked) |
| **URL** | Short, lowercase, hyphenated, descriptive, stable. Plan the URL hierarchy (`/vehicles/car-finance`) before launch, because changing URLs later needs redirects. **The home page should live at the site root `/`.** A separate `/home` URL splits signals and needs a canonical or redirect | The site root `/` returns **404 Not Found** (checked); Home lives at `/home` |
| **Operation: Create** | New pages need to be discoverable: linked from navigation or other pages, listed in the XML sitemap, and indexable (not blocked) | No sitemap exists (checked) |
| **Operation: Edit** | If the URL, title, main heading or core content changes, rankings can move. Changing a URL requires a **301 redirect** from the old one. Record what changed and when, so ranking shifts can be explained | No redirect mechanism yet |
| **Operation: Remove** | Removing a page that has rankings or backlinks loses them unless it is **301-redirected** to the closest relevant page. Truly gone content can return 410. All internal links to it must be updated | The remove flow lists files and references, but nothing about redirects |
| **What the page is for** | One **primary search intent** and keyword focus per page. Two pages targeting the same thing compete with each other (**keyword cannibalisation**) | Home's six vehicle tiles and the Vehicles page's six finance cards cover the same topics with similar copy: a cannibalisation risk once real copy is written |
| **In top bar?** | Pages in the main nav get the most internal-link weight. Important commercial pages should be in the nav or one click from it | Vehicles is in the nav |
| **Out of scope for this request** | Links to pages "not built yet" become **404s**. Better: don't link to them until they exist, or point the link somewhere real for now | Brief guidance now covers this |

### Brief Part 5 · Sections

| Brief item | SEO factors to consider | Status today |
|---|---|---|
| **Section order** | Content near the top of the page and in the first `h1`/`h2` signals the page's main topic. Put the core answer to the searcher's intent early, not below several promotional bands | Hero, then tiles, statement, calculator, steps, reviews, closing call to action |
| **Section type: Hero** | Holds the page's **only `h1`**. The headline should state the topic in plain terms a searcher would use; clever slogans make poor `h1`s. The intro line can carry secondary terms. Usually the **LCP element** (the largest thing on screen at load), so it must load fast | `h1` "Vehicle finance without the runaround": topic-led enough. Load speed affected by the build issue in part 3 |
| **Section type: Tile grid** | Tiles are thin content (title plus one line). Their value is **internal linking**: each tile should link to a dedicated page with descriptive anchor text | All six tiles link to the calculator, not to topic pages |
| **Section type: Statement and features** | Good place for unique value-proposition content. Feature titles become `h3`s: make them descriptive | Placeholder descriptions |
| **Section type: Card grid** | Can carry real depth (description plus points). Each card is a candidate for its own page. If cards stay on one page, they need enough unique text to justify the page | Placeholder content |
| **Section type: Numbered steps** | Process content answers "how does it work" queries well. Rich results for step lists have been largely retired, so value is in clear, specific text | Placeholder descriptions |
| **Section type: Calculator** | Calculators and tools earn links and engagement, and can rank for "… calculator" queries if they have a **crawlable URL and explanatory text** around them (how it works, assumptions, worked example). Results computed in the browser are not indexed; the explanation is | Calculator sits mid-page on Home; no dedicated URL or explanatory content |
| **Section type: Reviews** | Only genuine reviews with real attribution. Fake reviews breach Google policy and consumer law. **Review stars in search results are not available for reviews a business publishes about itself on its own site**, so do not plan around review schema for these. Consider linking to independent review platforms | Mock reviews, clearly labelled placeholders |
| **Section type: Closing call to action** | Conversion section; minimal SEO value. Keep it short so it does not dilute page content | Fine |
| **Section type: Text block** | The main place for **indexable depth**: explanations, eligibility, FAQs, guides. Commercial pages often need several hundred words of genuinely useful content to compete | Not built yet |
| **Section name** | Internal folder name only, not visible. The anchor `id` on each section (`#calculator`) can enable jump links in search results if headings are clear | Ids exist on Home sections |
| **Eyebrow label** | Decorative label, correctly marked up as a paragraph rather than a heading. Keep it that way so heading structure stays clean | Correct (checked in code) |
| **Heading** | Section headings are `h2`s: descriptive, topic-relevant, in a logical outline. Avoid vague headings ("Ready when you are.") for sections that carry content | Several headings are slogan-style |
| **Intro line** | Supporting text; a natural place for secondary terms | — |
| **Buttons (label → target)** | Descriptive anchor text and real URL targets. "Calculate repayments" is good; "Learn more" repeated many times is weak | Most buttons target `/home#calculator` |
| **Copy: Real or Placeholder** | **Placeholder text must never be indexed.** Thin or templated text across pages harms the whole site's quality assessment. The development preview must not be publicly crawlable | All descriptive copy is placeholder |
| **Items (tiles, cards, features, steps, reviews)** | Count and depth: enough items to be useful, each with unique text. Avoid the same item text repeated across pages (Home tiles vs Vehicles cards) | Same six topics on two pages |
| **New section type** | A new type should state its content purpose. Prefer types that carry real text over decorative ones | — |
| **Missing section type: FAQ** | Not in the section library yet. FAQs answer long-tail questions well and are useful content, though FAQ rich results are now shown only for a small set of authoritative sites | Not available |

### Brief Part 6 · Forms and inputs

| Brief item | SEO factors to consider | Status today |
|---|---|---|
| **Field labels and input types** | Accessibility: every field needs a visible, programmatic label. Mobile usability: correct input types bring up the right keyboard (number pad for amounts) | Labels present |
| **Starts blank / pre-filled** | Pre-filled example values let a tool show a result immediately, which helps engagement. Blank fields mean an empty-looking tool on first view | Calculator starts blank |
| **Button label / what happens / result** | No direct SEO impact. Plan **conversion tracking** (analytics events for "calculated", "submitted") so SEO traffic can be tied to outcomes | No analytics in the build |
| **Disclaimer under the result** | Crawlable text, close to the figures. Required for finance trust and compliance | Present (checked) |
| **Pretend submit (development only)** | When forms go live: thank-you or confirmation pages should be **noindexed**, and form parameters must not create indexable duplicate URLs | Not applicable yet |

### Brief Part 7 · Anything else

Good place for the team to record:
- the old site's URLs, if a site replaces one, for redirect mapping;
- target keywords and competitor pages;
- analytics and Search Console requirements;
- launch date and indexing plan.

---

## 2. SEO information the brief does not ask for yet

Candidate new fields for the team to consider. Most are per page.

| Candidate field | Where it would go | Why |
|---|---|---|
| Primary keyword / topic and search intent | Part 4, per page | Drives title, `h1`, headings and copy; prevents cannibalisation |
| Secondary keywords | Part 4, per page | Guides section headings and copy |
| Title tag | Part 4, per page | Currently defaulted from the page name |
| Meta description | Part 4, per page | None exist today |
| Canonical URL | Part 4, per page | Needed wherever the same content can appear at more than one URL (`/` vs `/home`) |
| Index / noindex | Part 4, per page | Thank-you pages, test pages and thin pages should be noindexed |
| Social sharing image and text (Open Graph) | Part 2 (site default) and Part 4 (override) | Controls how links look when shared |
| Structured data type | Part 1 (Organization / LocalBusiness) and Part 4 (per page, e.g. FinancialProduct, FAQPage, BreadcrumbList) | Eligibility for rich results and clearer entity understanding |
| Breadcrumbs | Part 3 | Internal linking and breadcrumb display in results |
| Images and alt text | Part 5, per section | No images on built pages yet; when added, each needs alt text, size limits and modern formats |
| Internal links in and out | Part 4, per page | Which existing pages should link to this one, and which it should link to |
| Old URL → new URL redirect map | Part 4 (Edit/Remove) and Part 7 | Preserves rankings when URLs change or pages go |
| Author / reviewer and last-updated date | Part 4, for content pages | E-E-A-T, especially for YMYL finance content |
| Regulatory trust content | Part 1 | Licence numbers, complaints, privacy: required for finance sites |
| Location / service area | Part 1 | Local relevance and `LocalBusiness` details |
| Analytics events and conversion goals | Part 6 | Measuring what SEO traffic actually does |

---

## 3. SEO of the build itself

These are properties of **how pages are produced**, so they apply to every page regardless of brief answers. They need fixing in the build process (Claude's build instructions and the site template), not in the brief.

| Issue | Detail | Severity | Status |
|---|---|---|---|
| **Search engines receive almost no content** | Every component loads its content only *after* the page becomes interactive in the browser. The HTML the server first sends contains the page title and a single "Loading..." message: no headline, no sections, no links. Google can sometimes render JavaScript later, but other search engines, social previews and AI crawlers generally will not, and even Google's rendering is delayed and less reliable for this kind of app. **This is the most important SEO issue in the current setup.** The fix is to load content during server-side rendering (prerendering) so the first HTML response contains the full page | **Critical** | Verified on `/home` (checked) |
| No meta descriptions | Pages set a title but no description tag | High | Checked |
| Site root returns 404 | There is no page at `/`; Home is at `/home` | High | Checked |
| No `robots.txt` | Returns 404 | Medium | Checked |
| No XML sitemap | `/sitemap.xml` returns 404 | Medium | Checked |
| No canonical tags | Needed once `/` and `/home` both exist, or with any duplicate paths | Medium | Not present in the page template |
| Language set to `en`, not `en-AU` | Regional targeting signal | Low | Checked |
| Mobile viewport set to `initial-scale=0.8` | The page opens zoomed out on phones. This is unusual and can hurt mobile usability and text legibility | Medium | Checked in the page template |
| Heavy shared scripts on every page | The page template loads scripts and styles for PDF viewers, a Word document editor, a rich-text editor and a component-library theme on **every** page, including marketing pages that use none of them. This slows loading (Core Web Vitals) | Medium | Seen in the page template; not measured |
| Loading spinners then content | Each section shows a spinner and then its content, so the layout jumps as the page fills in. This affects Core Web Vitals (Cumulative Layout Shift, Largest Contentful Paint). It largely goes away with the prerendering fix | Medium | Observed |
| Live connection required | Blazor Server keeps a live connection per visitor for interactivity. Fine for tools like the calculator, but a heavier model for content pages than plain HTML. Worth a deliberate decision for high-traffic marketing sites | Decision | — |
| No structured data | No schema markup is generated | Medium | Checked in code |
| No analytics or Search Console set-up | Nothing in the template | Medium | — |
| Development preview must stay private | The Sandox preview runs on a local machine today. Any shared staging site must be noindexed or password-protected so placeholder content never gets indexed | Process | — |

---

## 4. Suggested priorities

1. **Fix content delivery to search engines** (prerendering with content). Every other on-page improvement depends on search engines being able to see the page.
2. **Add per-page SEO fields to the brief** (keyword and intent, title, meta description, canonical, index/noindex) and have the build process output them.
3. **Root URL, robots.txt, sitemap and canonical handling** as standard parts of every site.
4. **A Text block and an FAQ section type**, so pages can carry real depth, plus a rule against duplicate copy between pages.
5. **Finance trust content** (licence, complaints, privacy, about) as a required part of any finance brand's brief.
6. **Performance clean-up**: stop loading unused editor and viewer scripts on marketing pages; fix the mobile viewport.
