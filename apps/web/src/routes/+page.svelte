<script lang="ts">
  import { ReactionType, UserRole, EntityStatus } from '@commentable/shared-enums';
</script>

<div class="container mx-auto px-4 py-16">
  <div class="max-w-4xl mx-auto">
    <h1 class="text-5xl font-bold mb-4">
      C# SvelteKit Commentable Project
    </h1>

    <p class="text-xl text-muted-foreground mb-8">
      Production-ready MonoRepo with C# .NET 9 API and SvelteKit 5 Frontend
    </p>

    <div class="grid gap-6 md:grid-cols-2 mb-12">
      <div class="rounded-lg border bg-card p-6">
        <h2 class="text-2xl font-semibold mb-3">✅ Frontend Ready</h2>
        <ul class="space-y-2 text-muted-foreground">
          <li>✓ SvelteKit 5 with TypeScript</li>
          <li>✓ TailwindCSS configured</li>
          <li>✓ Shadcn-Svelte components</li>
          <li>✓ Dark mode support</li>
          <li>✓ Shared types imported</li>
        </ul>
      </div>

      <div class="rounded-lg border bg-card p-6">
        <h2 class="text-2xl font-semibold mb-3">🔨 API Setup Needed</h2>
        <ul class="space-y-2 text-muted-foreground">
          <li>• Create entity classes</li>
          <li>• Configure DbContext</li>
          <li>• Run migrations</li>
          <li>• Implement repositories</li>
          <li>• Create controllers</li>
        </ul>
      </div>
    </div>

    <div class="rounded-lg border bg-card p-6 mb-8">
      <h2 class="text-2xl font-semibold mb-4">Shared Enums Demo</h2>
      <p class="text-muted-foreground mb-4">
        Using <code class="px-2 py-1 bg-muted rounded">@commentable/shared-enums</code>
      </p>

      <div class="space-y-3">
        <div>
          <h3 class="text-sm font-medium mb-2">Reaction Types:</h3>
          <div class="flex flex-wrap gap-2">
            {#each Object.values(ReactionType) as type}
              <span class="px-3 py-1 rounded-full bg-secondary text-secondary-foreground text-sm">
                {type}
              </span>
            {/each}
          </div>
        </div>

        <div>
          <h3 class="text-sm font-medium mb-2">User Roles:</h3>
          <div class="flex flex-wrap gap-2">
            {#each Object.values(UserRole) as role}
              <span class="px-3 py-1 rounded-full bg-primary/10 text-primary text-sm">
                {role}
              </span>
            {/each}
          </div>
        </div>

        <div>
          <h3 class="text-sm font-medium mb-2">Entity Status:</h3>
          <div class="flex flex-wrap gap-2">
            {#each Object.values(EntityStatus) as status}
              <span class="px-3 py-1 rounded-full bg-accent text-accent-foreground text-sm">
                {status}
              </span>
            {/each}
          </div>
        </div>
      </div>
    </div>

    <div class="rounded-lg border bg-muted/50 p-6">
      <h3 class="font-semibold mb-2">Next Steps:</h3>
      <ol class="list-decimal list-inside space-y-1 text-sm text-muted-foreground">
        <li>Read <code>GETTING_STARTED.md</code> for detailed instructions</li>
        <li>Create C# entity classes in <code>apps/api/Domain/Entities/</code></li>
        <li>Run database migrations</li>
        <li>Start building API endpoints</li>
        <li>Create frontend components</li>
      </ol>
    </div>
  </div>
</div>
