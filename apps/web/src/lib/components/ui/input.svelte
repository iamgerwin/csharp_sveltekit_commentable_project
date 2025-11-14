<script lang="ts">
	import { cn } from '$lib/utils';

	interface InputProps {
		type?: string;
		value?: string;
		placeholder?: string;
		disabled?: boolean;
		required?: boolean;
		class?: string;
		id?: string;
		name?: string;
		min?: string | number;
		max?: string | number;
		step?: string | number;
		'aria-label'?: string;
		'aria-describedby'?: string;
		'aria-invalid'?: boolean;
		'aria-required'?: boolean;
		autocomplete?: string;
		oninput?: (e: Event) => void;
		onchange?: (e: Event) => void;
		onblur?: (e: FocusEvent) => void;
		onfocus?: (e: FocusEvent) => void;
	}

	let {
		type = 'text',
		value = $bindable(''),
		placeholder,
		disabled = false,
		required = false,
		class: className,
		id,
		name,
		'aria-label': ariaLabel,
		'aria-describedby': ariaDescribedby,
		'aria-invalid': ariaInvalid,
		'aria-required': ariaRequired,
		autocomplete,
		oninput,
		onchange,
		onblur,
		onfocus,
		...restProps
	}: InputProps = $props();
</script>

<input
	{type}
	{id}
	{name}
	{placeholder}
	{disabled}
	{required}
	bind:value
	class={cn(
		'flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50',
		className
	)}
	aria-label={ariaLabel}
	aria-describedby={ariaDescribedby}
	aria-invalid={ariaInvalid}
	aria-required={ariaRequired || required}
	autocomplete={autocomplete}
	{oninput}
	{onchange}
	{onblur}
	{onfocus}
	{...restProps}
/>
