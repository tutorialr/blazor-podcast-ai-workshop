using Microsoft.Extensions.AI;

namespace Blazor.Podcast.AI;

public class Provider(IChatClient chat)
{
    // System Prompt
    private const string system = @"
    You are a friendly and useful assistant that will help with social media
    to create engaging posts adapted for main social media platforms also
    include any suitable hashtags, offer advice on content scheduling, 
    latest trends and any best practices to maximise visibility.
    Only use simple html and no markdown to format responses";

    // Cancellation
    private CancellationTokenSource? cancel;

    // Properties
    public string Title { get; } = "Social Media AI";

    public string Label { get; } = "Provide content for social media or questions";

    public Dictionary<string, List<string>> Questions { get; } = new()
    {       
        { "Podcast is about", [string.Empty] },
        { "Host of podcast is", [string.Empty] },
        { "Listener of podcast is", [string.Empty] },
        { "Format of podcast is", 
        ["Solo", "Interview", "Cohosted", "Roundtable", "Audiobook"] },
        { "Purpose of podcast is", 
        ["Community", "Discussion", "Education", "Experience", "Entertainment"] }
    };

    public List<ChatMessage> Messages { get; set; } = [new(ChatRole.System, system)];

    public bool IsQuestions { get; set; } = false;

    public bool IsGenerating { get; set; } = false;

    // Send Method
    public async Task Send(string message)
    {
        cancel = new();
        IsGenerating = true;
        IsQuestions = false;
                
        Messages.Add(new ChatMessage(ChatRole.User, message)); 
                
        var response = await chat.GetResponseAsync([.. Messages], null, cancel.Token);
        var assistant = new TextContent(response.Text);

        Messages.Add(new ChatMessage(ChatRole.Assistant, [assistant]));

        IsGenerating = false;
    }

    // Cancel & New Methods
    public void Cancel()
    {            
        cancel?.Cancel();
        IsGenerating = false;
    }

    public void New()
    {
        Cancel();
        IsQuestions = false;
        Messages = [new(ChatRole.System, system)];
    }
}