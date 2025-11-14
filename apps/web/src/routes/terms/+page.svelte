<script lang="ts">
	import { _ } from 'svelte-i18n';
	import Card from '$lib/components/ui/card.svelte';

	const lastUpdated = '2025-01-14';
</script>

<svelte:head>
	<title>Terms of Service - Commentable</title>
	<meta name="description" content="Terms of Service and User Agreement" />
</svelte:head>

<div class="max-w-4xl mx-auto px-4 py-8" id="main-content">
	<Card class="p-8">
		<h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">
			Terms of Service
		</h1>

		<p class="text-sm text-gray-600 dark:text-gray-400 mb-8">
			Last Updated: {lastUpdated}
		</p>

		<div class="prose dark:prose-invert max-w-none space-y-6">
			<!-- Introduction -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">1. Acceptance of Terms</h2>
				<p class="text-gray-700 dark:text-gray-300">
					By accessing or using Commentable ("Service"), you agree to be bound by these Terms of Service
					("Terms"). If you do not agree to these Terms, you may not access or use the Service. We reserve
					the right to modify these Terms at any time, and your continued use constitutes acceptance of any changes.
				</p>
			</section>

			<!-- Eligibility -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">2. Eligibility</h2>
				<p class="text-gray-700 dark:text-gray-300">
					You must be at least 13 years old (or 16 in the EU) to use this Service. By using the Service,
					you represent and warrant that you meet this age requirement. If you are under 18, you represent
					that you have your parent or guardian's permission to use the Service.
				</p>
			</section>

			<!-- User Accounts -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">3. User Accounts</h2>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">3.1 Account Creation</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					You must provide accurate, complete, and current information during registration. You are
					responsible for maintaining the confidentiality of your account credentials and for all activities
					under your account.
				</p>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">3.2 Account Security</h3>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li>You must immediately notify us of any unauthorized use of your account</li>
					<li>You are solely responsible for all activity on your account</li>
					<li>We are not liable for any loss or damage from your failure to protect your credentials</li>
				</ul>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">3.3 Account Termination</h3>
				<p class="text-gray-700 dark:text-gray-300">
					We reserve the right to suspend or terminate your account at any time for violations of these
					Terms, illegal activity, or any other reason at our sole discretion.
				</p>
			</section>

			<!-- User Content -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">4. User-Generated Content</h2>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">4.1 Your Content</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					You retain ownership of all content you post, upload, or share on the Service ("User Content").
					By posting User Content, you grant us a worldwide, non-exclusive, royalty-free license to use,
					reproduce, modify, distribute, and display your content in connection with operating the Service.
				</p>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">4.2 Content Restrictions</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">You agree NOT to post content that:</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li>Violates any law or regulation</li>
					<li>Infringes on intellectual property rights of others</li>
					<li>Contains hate speech, harassment, or threats</li>
					<li>Contains sexually explicit, violent, or graphic content</li>
					<li>Promotes illegal activities or violence</li>
					<li>Contains malware, viruses, or malicious code</li>
					<li>Impersonates any person or entity</li>
					<li>Contains spam or unsolicited advertising</li>
				</ul>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">4.3 Content Moderation</h3>
				<p class="text-gray-700 dark:text-gray-300">
					We reserve the right (but have no obligation) to review, monitor, and remove any User Content
					that violates these Terms or is otherwise objectionable, at our sole discretion and without notice.
				</p>
			</section>

			<!-- Intellectual Property -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">5. Intellectual Property</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					The Service and its original content (excluding User Content), features, and functionality are
					owned by Commentable and are protected by international copyright, trademark, patent, trade secret,
					and other intellectual property laws.
				</p>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">5.1 Copyright Infringement</h3>
				<p class="text-gray-700 dark:text-gray-300">
					If you believe your copyright has been infringed, please contact us at
					<strong>copyright@commentable.com</strong> with:
				</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li>Description of the copyrighted work</li>
					<li>Location of the infringing material on our Service</li>
					<li>Your contact information</li>
					<li>A statement of good faith belief that use is unauthorized</li>
					<li>A statement that the information is accurate and you are authorized to act</li>
				</ul>
			</section>

			<!-- Prohibited Conduct -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">6. Prohibited Conduct</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">You agree NOT to:</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li>Use the Service for any unlawful purpose</li>
					<li>Attempt to gain unauthorized access to our systems</li>
					<li>Interfere with or disrupt the Service or servers</li>
					<li>Use automated systems (bots, scrapers) without permission</li>
					<li>Collect user information without consent</li>
					<li>Reverse engineer or decompile any part of the Service</li>
					<li>Bypass any security measures or rate limiting</li>
					<li>Create multiple accounts to evade enforcement actions</li>
				</ul>
			</section>

			<!-- Privacy -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">7. Privacy</h2>
				<p class="text-gray-700 dark:text-gray-300">
					Your privacy is important to us. Please review our
					<a href="/privacy" class="text-primary hover:underline">Privacy Policy</a> to understand how
					we collect, use, and protect your personal information. By using the Service, you consent to
					our Privacy Policy.
				</p>
			</section>

			<!-- Disclaimers -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">8. Disclaimers</h2>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">8.1 "AS IS" Service</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					THE SERVICE IS PROVIDED "AS IS" AND "AS AVAILABLE" WITHOUT WARRANTIES OF ANY KIND, EITHER
					EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO WARRANTIES OF MERCHANTABILITY, FITNESS FOR
					A PARTICULAR PURPOSE, AND NON-INFRINGEMENT.
				</p>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">8.2 No Guarantee</h3>
				<p class="text-gray-700 dark:text-gray-300">
					We do not guarantee that the Service will be uninterrupted, secure, or error-free. We do not
					warrant that results obtained from using the Service will be accurate or reliable.
				</p>
			</section>

			<!-- Limitation of Liability -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">9. Limitation of Liability</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					TO THE MAXIMUM EXTENT PERMITTED BY LAW, COMMENTABLE SHALL NOT BE LIABLE FOR ANY INDIRECT,
					INCIDENTAL, SPECIAL, CONSEQUENTIAL, OR PUNITIVE DAMAGES, OR ANY LOSS OF PROFITS OR REVENUES,
					WHETHER INCURRED DIRECTLY OR INDIRECTLY, OR ANY LOSS OF DATA, USE, GOODWILL, OR OTHER
					INTANGIBLE LOSSES.
				</p>
				<p class="text-gray-700 dark:text-gray-300">
					OUR TOTAL LIABILITY TO YOU FOR ALL CLAIMS ARISING FROM OR RELATED TO THE SERVICE SHALL NOT
					EXCEED THE AMOUNT YOU PAID US IN THE PAST 12 MONTHS, OR $100 USD, WHICHEVER IS GREATER.
				</p>
			</section>

			<!-- Indemnification -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">10. Indemnification</h2>
				<p class="text-gray-700 dark:text-gray-300">
					You agree to indemnify, defend, and hold harmless Commentable, its officers, directors,
					employees, and agents from any claims, liabilities, damages, losses, and expenses (including
					legal fees) arising from:
				</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li>Your violation of these Terms</li>
					<li>Your User Content</li>
					<li>Your violation of any third-party rights</li>
					<li>Your use or misuse of the Service</li>
				</ul>
			</section>

			<!-- Dispute Resolution -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">11. Dispute Resolution</h2>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">11.1 Informal Resolution</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					Before filing a claim, you agree to contact us at <strong>legal@commentable.com</strong> to
					attempt to resolve the dispute informally for at least 30 days.
				</p>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">11.2 Arbitration</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					Any dispute that cannot be resolved informally shall be resolved through binding arbitration
					in accordance with the rules of [Arbitration Association]. Arbitration will be conducted in
					[Location] and judgments may be entered in any court of competent jurisdiction.
				</p>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">11.3 Class Action Waiver</h3>
				<p class="text-gray-700 dark:text-gray-300">
					You agree to resolve disputes individually and waive the right to participate in class actions,
					class arbitrations, or representative actions.
				</p>
			</section>

			<!-- Governing Law -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">12. Governing Law</h2>
				<p class="text-gray-700 dark:text-gray-300">
					These Terms shall be governed by and construed in accordance with the laws of [Jurisdiction],
					without regard to its conflict of law provisions. You consent to the exclusive jurisdiction of
					the courts in [Location] for any disputes.
				</p>
			</section>

			<!-- Changes to Terms -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">13. Changes to Terms</h2>
				<p class="text-gray-700 dark:text-gray-300">
					We reserve the right to modify these Terms at any time. We will notify you of material changes
					by email or through the Service. Your continued use after changes constitutes acceptance of the
					modified Terms. If you do not agree, you must stop using the Service.
				</p>
			</section>

			<!-- Severability -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">14. Severability</h2>
				<p class="text-gray-700 dark:text-gray-300">
					If any provision of these Terms is found to be unenforceable or invalid, that provision will be
					limited or eliminated to the minimum extent necessary, and the remaining provisions will remain
					in full force and effect.
				</p>
			</section>

			<!-- Entire Agreement -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">15. Entire Agreement</h2>
				<p class="text-gray-700 dark:text-gray-300">
					These Terms, along with our Privacy Policy, constitute the entire agreement between you and
					Commentable regarding the Service and supersede all prior agreements and understandings.
				</p>
			</section>

			<!-- Contact -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">16. Contact Us</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					If you have questions about these Terms, contact us:
				</p>
				<div class="bg-gray-50 dark:bg-gray-800 p-4 rounded-lg">
					<p class="text-gray-700 dark:text-gray-300">
						<strong>Email:</strong> legal@commentable.com<br>
						<strong>Support:</strong> support@commentable.com<br>
						<strong>Address:</strong> [Your Business Address]
					</p>
				</div>
			</section>
		</div>
	</Card>
</div>
