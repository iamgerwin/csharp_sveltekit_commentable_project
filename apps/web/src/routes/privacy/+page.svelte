<script lang="ts">
	import { _ } from 'svelte-i18n';
	import Card from '$lib/components/ui/card.svelte';

	const lastUpdated = '2025-01-14';
</script>

<svelte:head>
	<title>Privacy Policy - Commentable</title>
	<meta name="description" content="Privacy Policy and Data Protection Information" />
</svelte:head>

<div class="max-w-4xl mx-auto px-4 py-8" id="main-content">
	<Card class="p-8">
		<h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">
			Privacy Policy
		</h1>

		<p class="text-sm text-gray-600 dark:text-gray-400 mb-8">
			Last Updated: {lastUpdated}
		</p>

		<div class="prose dark:prose-invert max-w-none space-y-6">
			<!-- Introduction -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">1. Introduction</h2>
				<p class="text-gray-700 dark:text-gray-300">
					Commentable ("we," "our," or "us") is committed to protecting your privacy and personal data.
					This Privacy Policy explains how we collect, use, disclose, and safeguard your information when
					you use our platform. This policy complies with the General Data Protection Regulation (GDPR),
					California Consumer Privacy Act (CCPA), and other applicable data protection laws.
				</p>
			</section>

			<!-- Data Collection -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">2. Information We Collect</h2>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">2.1 Personal Information</h3>
				<p class="text-gray-700 dark:text-gray-300 mb-3">We collect the following personal information:</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li><strong>Account Information:</strong> Username, email address, password (encrypted)</li>
					<li><strong>Profile Information:</strong> User role, account creation date</li>
					<li><strong>Content:</strong> Videos, posts, comments, and reactions you create</li>
					<li><strong>Usage Data:</strong> IP address, browser type, device information, access times</li>
				</ul>

				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3 mt-4">2.2 Cookies and Tracking</h3>
				<p class="text-gray-700 dark:text-gray-300">
					We use cookies and similar technologies to enhance your experience. You can manage your
					cookie preferences through our cookie consent banner. See our Cookie Policy for details.
				</p>
			</section>

			<!-- How We Use Data -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">3. How We Use Your Information</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">We use your personal data for the following purposes:</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li><strong>Service Delivery:</strong> To provide, maintain, and improve our platform</li>
					<li><strong>Authentication:</strong> To verify your identity and manage your account</li>
					<li><strong>Communication:</strong> To send you service updates and notifications</li>
					<li><strong>Security:</strong> To detect, prevent, and address fraud or security issues</li>
					<li><strong>Compliance:</strong> To comply with legal obligations and enforce our terms</li>
					<li><strong>Analytics:</strong> To understand usage patterns and improve our services (with consent)</li>
				</ul>
			</section>

			<!-- Legal Basis (GDPR) -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">4. Legal Basis for Processing (GDPR)</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">Under GDPR, we process your data based on:</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li><strong>Contract Performance:</strong> Processing necessary to provide our services</li>
					<li><strong>Consent:</strong> You have given explicit consent (e.g., cookies, marketing)</li>
					<li><strong>Legitimate Interest:</strong> For security, fraud prevention, and service improvement</li>
					<li><strong>Legal Obligation:</strong> Compliance with laws and regulations</li>
				</ul>
			</section>

			<!-- Data Sharing -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">5. Data Sharing and Disclosure</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					We do not sell your personal information. We may share your data with:
				</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li><strong>Service Providers:</strong> Third parties that help us operate our platform (hosting, analytics)</li>
					<li><strong>Legal Authorities:</strong> When required by law or to protect rights and safety</li>
					<li><strong>Business Transfers:</strong> In connection with a merger, acquisition, or asset sale</li>
					<li><strong>With Your Consent:</strong> When you explicitly authorize data sharing</li>
				</ul>
			</section>

			<!-- Data Security -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">6. Data Security</h2>
				<p class="text-gray-700 dark:text-gray-300">
					We implement industry-standard security measures including:
				</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li>TLS/HTTPS encryption for data in transit</li>
					<li>Bcrypt password hashing with salt</li>
					<li>Access controls and authentication</li>
					<li>Regular security audits and monitoring</li>
					<li>Audit logging for compliance (HIPAA-compliant)</li>
				</ul>
			</section>

			<!-- Data Retention -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">7. Data Retention</h2>
				<p class="text-gray-700 dark:text-gray-300">
					We retain your personal data for as long as necessary to provide our services and comply
					with legal obligations. When you delete your account, we will anonymize or delete your
					personal information within 30 days, except where retention is required by law.
				</p>
			</section>

			<!-- Your Rights -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">8. Your Rights</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					Under GDPR and other privacy laws, you have the following rights:
				</p>
				<ul class="list-disc pl-6 text-gray-700 dark:text-gray-300 space-y-2">
					<li><strong>Right to Access:</strong> Request a copy of your personal data (use our data export feature)</li>
					<li><strong>Right to Rectification:</strong> Correct inaccurate or incomplete data</li>
					<li><strong>Right to Erasure:</strong> Request deletion of your account and data ("right to be forgotten")</li>
					<li><strong>Right to Restriction:</strong> Limit how we use your data</li>
					<li><strong>Right to Data Portability:</strong> Receive your data in a machine-readable format</li>
					<li><strong>Right to Object:</strong> Object to data processing for marketing or research</li>
					<li><strong>Right to Withdraw Consent:</strong> Withdraw consent at any time</li>
				</ul>
				<p class="text-gray-700 dark:text-gray-300 mt-3">
					To exercise these rights, contact us at privacy@commentable.com or use the in-app data
					export and account deletion features.
				</p>
			</section>

			<!-- Children's Privacy -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">9. Children's Privacy</h2>
				<p class="text-gray-700 dark:text-gray-300">
					Our platform is not intended for children under 13 years of age (or 16 in the EU).
					We do not knowingly collect personal information from children. If you believe we have
					collected data from a child, please contact us immediately.
				</p>
			</section>

			<!-- International Transfers -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">10. International Data Transfers</h2>
				<p class="text-gray-700 dark:text-gray-300">
					Your data may be transferred to and processed in countries outside your residence.
					We ensure appropriate safeguards are in place, including Standard Contractual Clauses (SCCs)
					and adequacy decisions, to protect your data.
				</p>
			</section>

			<!-- Changes to Policy -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">11. Changes to This Policy</h2>
				<p class="text-gray-700 dark:text-gray-300">
					We may update this Privacy Policy from time to time. We will notify you of material changes
					by email or through our platform. Your continued use after changes constitutes acceptance of
					the updated policy.
				</p>
			</section>

			<!-- Contact -->
			<section>
				<h2 class="text-2xl font-semibold text-gray-900 dark:text-white mb-4">12. Contact Us</h2>
				<p class="text-gray-700 dark:text-gray-300 mb-3">
					If you have questions or concerns about this Privacy Policy or our data practices, contact us:
				</p>
				<div class="bg-gray-50 dark:bg-gray-800 p-4 rounded-lg">
					<p class="text-gray-700 dark:text-gray-300">
						<strong>Email:</strong> privacy@commentable.com<br>
						<strong>Data Protection Officer:</strong> dpo@commentable.com<br>
						<strong>Address:</strong> [Your Business Address]
					</p>
				</div>
				<p class="text-gray-700 dark:text-gray-300 mt-3">
					<strong>EU Representative:</strong> If you are in the EU, you have the right to lodge a
					complaint with your local supervisory authority.
				</p>
			</section>
		</div>
	</Card>
</div>
