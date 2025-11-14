<script lang="ts">
	import { toastStore, type Toast } from '$lib/stores/toast.store.svelte';
	import { fly } from 'svelte/transition';

	const icons = {
		success: '✓',
		error: '✕',
		warning: '⚠',
		info: 'ℹ'
	};

	const colors = {
		success: 'bg-green-500',
		error: 'bg-red-500',
		warning: 'bg-yellow-500',
		info: 'bg-blue-500'
	};
</script>

<div class="fixed bottom-4 right-4 z-50 flex flex-col gap-2 max-w-sm">
	{#each toastStore.toasts as toast (toast.id)}
		<div
			transition:fly={{ x: 300, duration: 200 }}
			class="flex items-center gap-3 rounded-lg {colors[
				toast.type
			]} text-white px-4 py-3 shadow-lg"
		>
			<span class="text-lg font-bold">{icons[toast.type]}</span>
			<p class="flex-1 text-sm">{toast.message}</p>
			<button
				onclick={() => toastStore.remove(toast.id)}
				class="text-white/80 hover:text-white transition-colors"
			>
				✕
			</button>
		</div>
	{/each}
</div>
