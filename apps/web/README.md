# CommentableWeb - SvelteKit Frontend

Modern, responsive frontend built with SvelteKit 5, TypeScript, TailwindCSS, and Shadcn-Svelte components.

## Tech Stack

- **SvelteKit 5** - Latest version with runes
- **TypeScript** - Type safety
- **TailwindCSS** - Utility-first CSS framework with custom theme
- **Shadcn-Svelte** - Beautiful, accessible component library
- **Bits-UI** - Headless component primitives
- **Mode Watcher** - Dark/light mode support

## Project Structure

```
apps/web/src/
├── lib/
│   ├── components/ui/    # Shadcn-Svelte components
│   ├── api/              # API client functions
│   ├── stores/           # Svelte stores
│   ├── utils.ts          # Utility functions
│   └── assets/           # Static assets
├── routes/               # SvelteKit routes
├── app.css               # Global CSS with Tailwind
└── app.html              # HTML template
```

## Development

```bash
# Start dev server
npm run dev

# Or from project root
cd ../..
./run.sh web

# Build for production
npm run build

# Preview production build
npm run preview
```

## Adding Shadcn-Svelte Components

```bash
# Add UI components
npx shadcn-svelte@latest add button card dialog input

# List available components
npx shadcn-svelte@latest add
```

## Features

✅ TailwindCSS configured with custom theme
✅ Dark mode support with mode-watcher
✅ Shadcn-Svelte component library ready
✅ TypeScript with strict mode
✅ Utility functions (cn, flyAndScale)
✅ Responsive design utilities
✅ Custom CSS variables for theming

## Environment Variables

Create `.env` file:

```env
PUBLIC_API_URL=http://localhost:5000/api
```

## Resources

- [SvelteKit Docs](https://kit.svelte.dev/docs)
- [Shadcn-Svelte](https://shadcn-svelte.com/)
- [TailwindCSS Docs](https://tailwindcss.com/docs)
- [Project Documentation](../../docs/README.md)

For complete guides, examples, and API integration instructions, see the main project documentation.
