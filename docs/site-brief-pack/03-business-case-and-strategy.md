# Business Case and Strategy: Automated Website Builds

**Site brief pack · Document 3 of 3** · as at 20 October 2026

| Document | What it is |
|---|---|
| 1. Site Brief Template | The form a team member fills in to ask for a website, or a change to one |
| 2. SEO Factors by Brief Item | What each answer in the brief means for search performance |
| **3. Business Case and Strategy** (this one) | Why we are building this process, how it works, what has been done so far, and what is still open |

If you are new to the project, start here.

---

## 1. Summary

We are building a repeatable process for creating websites for **our own brands**, in which:

1. a team member fills in a structured **site brief**;
2. **Claude**, an AI assistant working directly inside our development project, turns the brief into working website pages, following a fixed set of written build rules;
3. the team reviews a **short build plan** before anything is built, and a **live preview** afterwards;
4. the pages are real application code in the same technology and structure our developers already use (**Blazor**, following the component pattern of our CRM platform), so the code that is reviewed is the code that ships.

The aim is to launch and change brand sites much faster and more consistently than designing and hand-coding each page, without giving up control of quality, compliance or SEO.

The process is about a week old. It has produced one test site (**JJ's Vehicle Finance**: a Home page and a Vehicles page) and a first version of the brief. The building blocks work. The biggest open item is SEO readiness: as built today, **search engines receive almost no page content**. Fixing that is a precondition for using this process for any public site (see section 7).

---

## 2. The problem we are solving

Each of our brands needs its own website, and those sites need frequent new pages and changes: new products, landing pages, campaigns, compliance updates. Building them the traditional way means:

- **Slow turnaround.** Every page goes through design, build and review cycles, often with developers as the bottleneck.
- **Inconsistency.** Each site and page is built a little differently, which makes them harder to maintain and harder to keep compliant.
- **Knowledge in people's heads.** What a page should contain, how it should look and what it must not say gets decided in meetings and messages, not recorded anywhere reusable.
- **SEO bolted on afterwards.** Search requirements arrive after pages are designed, when they are most expensive to change.

## 3. Goals

| Goal | What success looks like |
|---|---|
| **Speed** | A new page or site goes from request to reviewable preview within a single working session, not a design-and-build cycle |
| **Consistency** | Every page is built from the same small set of section types and the same code structure, so any developer can maintain any site |
| **Brand flexibility** | Each brand has its own look (colours, type, shapes) without changing the build process. The process itself has no built-in look |
| **Control** | Nothing is built without an agreed plan. Destructive changes (removing pages or files) always need explicit confirmation |
| **Compliance by default** | Rules such as "no invented rates or reviews" and "money figures carry a disclaimer" are built into the process, not left to memory. This matters for finance brands, which are held to a higher standard |
| **SEO by design** | Search requirements are captured in the brief and produced by the build, so every page is born search-ready (not yet achieved; see section 7) |
| **Production-grade output** | The output is the real code that will run in production on our Blazor platform, not a mock-up to be rebuilt |

## 4. How the process works

### 4.1 The workflow

```
 Team member                   Claude                          Team / developer
 ───────────                   ──────                          ────────────────
 1. Fill in the site brief  →  2. Read it, ask about gaps
                               3. Post a short build plan   →  4. Approve or change it
                               5. Build the pages
                               6. Check they compile, then
                                  open a live preview       →  7. Review in the browser
                                                               8. Request changes (back to 1 or 3)
                                                               9. Move to production
```

Steps 1 to 7 work today. Step 9 (moving to production) is not defined yet.

### 4.2 The pieces

| Piece | What it is | Where it lives |
|---|---|---|
| **Site brief** | The structured request: the site, its brand, navigation and footer, pages, the sections on each page, and any forms. A team member fills it in; blanks become questions | Template and example: document 1 |
| **Build instructions ("the skill")** | Written rules Claude follows every time: how files and folders are named, how a page is structured, which questions must be asked, how plans are presented, how forms and validation work, and what is never allowed (e.g. invented data, fixed colours) | `.claude/skills/website-component-generator/` in the project |
| **Section library** | The standard building blocks a page is made of: Hero, Tile grid, Statement and features, Card grid, Numbered steps, Calculator, Reviews, Closing call to action, Text block (planned). Plus Navigation and Footer on every page. Briefs pick from this list instead of designing layouts from scratch | `sections.md` in the skill folder |
| **Sandox** | The development project where pages are built and previewed. It is a stand-alone copy of our platform's front-end structure with **no database and no live connections**. Content is placeholder ("mock") data and form submissions are pretended, so anything can be built and tested safely | `C:\Projects\Sandox\Sandox` |
| **Shared components** | The ready-made inputs (text boxes, money fields, dropdowns, date pickers), pop-ups, tables and graphs, taken from our CRM. They automatically take on each brand's accent colour | `Components/Domains/Utlility/` in Sandox |

### 4.3 How a page is put together

Every page follows the same structure, which makes pages predictable for developers and for Claude:

```
Page (e.g. Home, at /home)
├── Navigation        the top bar
├── Sections          the page content, top to bottom
│   ├── Hero
│   ├── Tile grid
│   ├── Calculator
│   └── ...
└── Footer
```

Each block is a separate component following the CRM's established three-part code pattern:
- the **component**: what is shown and how it behaves;
- its **requests**: where its data comes from (placeholder data in Sandox, real data later);
- its **state**: anything shared with other components.

Because the data source is isolated in the requests part, moving a page to real data changes that part only, not the page itself.

### 4.4 How branding works

The build process deliberately contains **no styling**. Each brand supplies its own in the brief:

- a **palette**: Main, Accent, Dark, Tint, Grey, Error and Success colours;
- **shape and type** choices: heading style, button shape, corner rounding;
- a **colour roles table** saying which colour each part of a page uses (top bar, hero, buttons, cards, error messages...).

The shared form components follow the brand's accent automatically, through one setting per brand. Two brands can therefore look completely different while being built by identical rules and identical code structure.

## 5. What has been built so far

### 5.1 Timeline

| When | What |
|---|---|
| Before 13 Oct 2026 | Sandox project set up: a stand-alone copy of the CRM front-end structure, the shared input and pop-up components, a demo page, and the first version of Claude's build instructions (based on the CRM's component pattern) |
| 13 Oct 2026 | **Build rules tightened.** Every request must name the page and whether it is being created, edited or removed. All questions use a clickable question format. Build plans must be approved before building |
| 13 Oct 2026 | **First site page.** JJ's Vehicle Finance Home page with a working repayment calculator, built and verified in a browser preview (the calculator's figures were checked against the standard loan formula) |
| 13 Oct 2026 | **Sandox made self-contained.** It depended on a private company package that could not be installed on the development machine. The parts the pages need were rewritten inside the project (same names and behaviour as far as could be determined), the package was removed, and one component that relied on it (a Google address search) was deleted. Sandox now builds using only public packages |
| 13 Oct 2026 | **Page structure standardised.** Every page split into Navigation / Sections / Footer, with a fixed naming scheme. Home restructured into it and redesigned in a modern marketing style, using a public finance site's layout (not its branding or wording) as a structural reference. Node.js installed so the styling can be rebuilt and checked locally |
| 13 Oct 2026 | **Second page.** Vehicles page, with Home's top bar linking to it |
| 20 Oct 2026 | **Brief pack.** Site brief template, a completed example (JJ's), and the section library |
| 20 Oct 2026 | **Styling removed from the build rules.** All built-in colours and type sizes replaced with named roles, so the rules work for any brand. Brief extended with a palette and colour roles table |
| 20 Oct 2026 | **Brand-aware shared components.** The 34 shared component files (143 colour references) switched from a fixed indigo/blue to a per-brand accent colour. JJ's set to amber and verified in the preview |
| 20 Oct 2026 | **This document pack**, including an SEO review of the brief and of the build output |

### 5.2 Current state

| Area | State |
|---|---|
| Build rules | In place and in use; updated as decisions are made |
| Brief | Version 1 template plus a worked example; SEO fields not yet included (see document 2) |
| Section library | 8 section types built and documented; Text block planned; FAQ suggested |
| Test site | JJ's Vehicle Finance: Home (7 sections plus calculator) and Vehicles (2 sections). All copy is placeholder |
| Preview and checking | Each build is compiled and opened in a browser preview using a throwaway copy of the project, so a developer's open copy is not disturbed |
| SEO | Basic structure is sound (one main heading per page, logical sub-headings), but search engines currently receive almost no content, and standard SEO elements (descriptions, sitemap, robots file, root URL) are missing |
| Production path | Not defined |

## 6. Benefits

Described qualitatively; no measurements have been taken yet.

- **Faster delivery.** A page that would need design, build and review rounds is produced and previewed in one session, and changes take minutes.
- **Lower specialist dependency.** Marketing and SEO team members can request pages directly through the brief; developers review instead of hand-building.
- **Consistency at scale.** Every brand site uses the same structure and section types, so learning one site means knowing them all, and fixes to a section type can be applied everywhere.
- **Decisions are recorded.** The brief and the build plan form a written record of what was asked for and agreed.
- **Safer experimentation.** Sandox has no live data or connections, so pages can be tried out with no risk to customers or systems.
- **Compliance and SEO become part of the request** instead of a late review step (SEO part still to be completed).
- **Real code, not prototypes.** Output is in our production technology and structure, so nothing has to be rebuilt.

## 7. Risks, gaps and how we propose to handle them

| Risk / gap | Why it matters | Proposed handling |
|---|---|---|
| **Search engines see almost no content** | Pages load their content only after they become interactive in a browser. The first HTML a crawler receives is a title and a "Loading..." message. For public marketing sites this undermines SEO entirely | Change the build pattern so content is included when the page is first rendered on the server. Top priority before any public launch (document 2, part 3) |
| **No route from Sandox to production** | Pages can be built and previewed, but how they are deployed, where they are hosted, and how they get real content is undecided | Define the production app, hosting, deployment steps and the content source (see section 8) |
| **Placeholder content** | All copy is placeholder. It must never be published or indexed | A content step in the process, plus a rule that previews shared beyond the team are private or noindexed |
| **Rewritten framework parts** | The core parts replaced on 13 Oct match the original package's names and methods, but their inner behaviour was written from scratch because the original source was not available | Compare against the original package source before production, or adopt the original if the production app uses it |
| **Navigation and footer copied per page** | Each page has its own copy, so a site-wide change must be repeated on every page, and copies can drift apart | Consider a shared per-site Navigation and Footer once the structure settles |
| **No version history** | The Sandox folder is not under version control (git), so changes and deletions cannot be rolled back | Put Sandox under git before the process is used more widely |
| **AI-generated code** | Claude follows written rules and verifies that code compiles and works in a preview, but a developer should still review code before production | Developer review as a fixed step before production |
| **Finance compliance (YMYL)** | Finance brands are held to higher standards by regulators and by search engines | Compliance limits in every brief; mandatory trust content (licence, complaints, privacy) for finance brands |
| **Blazor Server for public sites** | The technology keeps a live connection open per visitor. Excellent for interactive tools, heavier than plain pages for high-traffic content | Deliberate decision when the production set-up is defined; prerendering also reduces the impact |
| **Page weight** | The page template loads document viewers and editors on every page, including marketing pages | Separate, lighter template for public brand sites |

## 8. Open decisions

| Decision | Options / notes |
|---|---|
| Where new brand sites live during development | Side by side inside Sandox, or a copy of the Sandox set-up per site (the brief has a field for this) |
| Production hosting and deployment | Which production Blazor application(s) serve the brand sites, and how pages move there |
| Content source | Where real copy and data come from once pages leave Sandox (fixed in code, a content system, a database) |
| SEO fields in the brief | Which of the candidate fields in document 2 become required |
| Who approves | Who signs off the build plan, the preview, compliance and SEO before launch |
| Measurement | Which outcomes to track (time to launch, rankings, conversions) to evaluate the process |

## 9. Proposed next steps

1. **Team review of the brief and SEO list** (in progress): agree which SEO fields to add and how the brief should read for non-technical users.
2. **Fix content delivery for search engines** in the build pattern, then rebuild JJ's pages to prove it.
3. **Add standard SEO output** to every site: meta descriptions, canonical tags, root URL, robots file, sitemap, language, structured data basics.
4. **Add Text block and FAQ section types**, so pages can carry real depth.
5. **Put Sandox under version control.**
6. **Define the production route**: hosting, deployment, content source, approvals.
7. **Run a second brand through the full process** from a completed brief, to test it end to end and measure the time taken.

---

## Appendix A · Glossary

| Term | Meaning |
|---|---|
| Blazor | Microsoft's framework for building web applications in C#. Our CRM front end uses it, and so do these sites |
| Brief | The structured request form (document 1) |
| Build plan | The short summary Claude posts before building: pages, components, content, any proposed colour roles. It must be approved first |
| Claude | The AI assistant that reads briefs and builds the pages inside the development project |
| Component | A self-contained piece of a page (a section, the top bar, an input). Each has its own folder |
| Colour roles | Which brand colour each part of a page uses, set in the brief |
| Accent class | A single setting per brand that makes all shared inputs and pop-ups use that brand's accent colour |
| Mock data / placeholder | Made-up content used during development. Must be replaced before launch |
| Prerendering | Producing a page's full HTML on the server before it is sent, so search engines and slow devices see the content immediately |
| Sandox | The stand-alone development project where pages are built and previewed |
| Section / section type | One band of a page / a standard layout from the section library |
| Skill / build instructions | The written rules Claude follows for every build |
| YMYL | "Your Money or Your Life": topics such as finance and health that search engines hold to higher quality and trust standards |

## Appendix B · Where things are

| Item | Location |
|---|---|
| Development project | `C:\Projects\Sandox\Sandox` |
| Brand site pages | `Sandox/Components/Domains/Pages/<Page>/` (e.g. `Pages/Home`, `Pages/Vehicles`) |
| Shared components | `Sandox/Components/Domains/Utlility/` |
| Core framework parts (rewritten 13 Oct) | `Sandox/Framework/` |
| Brand colours and accent settings | `Sandox/wwwroot/input.css` |
| Claude's build instructions | `C:\Projects\Sandox\.claude\skills\website-component-generator\` (`SKILL.md` plus reference files, `sections.md`, the brief template and example) |
| This document pack | `C:\Projects\Sandox\docs\site-brief-pack\` |
