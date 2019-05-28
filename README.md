QuartzHostedService

Based on a post by Andrew Lock (https://andrewlock.net/using-scoped-services-inside-a-quartz-net-hosted-service-with-asp-net-core/) and the gist (https://gist.github.com/Antaris/9c3c097d31a90da279e3e6d78497a369) about handling Quartz jobs requiring Scoped services.

This example uses an IJob wrapper to capture the Scope (rather than the dictionary suggested in the gist). This tightly associates the Scope with the job avoiding the "type collision" possible with the dictionary mentioned in the gist comment.

See [ScopedJobFactory.cs]

Thoughts?
