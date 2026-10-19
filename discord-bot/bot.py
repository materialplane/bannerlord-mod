import os
import sys
import asyncio
import discord
from discord.ext import commands
from dotenv import load_dotenv

load_dotenv()

TOKEN = os.getenv("DISCORD_BOT_TOKEN")
if not TOKEN:
    print("Error: DISCORD_BOT_TOKEN not found in environment.")
    sys.exit(1)

intents = discord.Intents.default()
intents.guilds = True
intents.members = True
intents.message_content = True

bot = commands.Bot(command_prefix="!", intents=intents)

ROLES_CONFIG = [
    {"name": "👑 High King", "color": discord.Color.gold(), "hoist": True, "mentionable": True, "admin": True},
    {"name": "🛡️ Marshal", "color": discord.Color.red(), "hoist": True, "mentionable": True, "manage_guild": True},
    {"name": "⚔️ Vanguard (Tester)", "color": discord.Color.orange(), "hoist": True, "mentionable": True},
    {"name": "📜 Chronicler", "color": discord.Color.purple(), "hoist": True, "mentionable": True},
    {"name": "🏰 Lord", "color": discord.Color.blue(), "hoist": True, "mentionable": True},
    {"name": "🌾 Wanderer", "color": discord.Color.light_grey(), "hoist": False, "mentionable": False},
]

STRUCTURE = [
    {
        "category": "📢 WELCOME & NOTICE BOARD",
        "channels": [
            {"name": "welcome-and-info", "topic": "Overview of Realms of Dust, rules, and community links.", "read_only": True},
            {"name": "announcements", "topic": "Official Realms of Dust announcements and major releases.", "read_only": True},
            {"name": "dev-log", "topic": "Development updates, balance snapshots, and roadmap progress.", "read_only": True},
            {"name": "patch-notes", "topic": "Detailed changelog and build versions.", "read_only": True},
        ]
    },
    {
        "category": "💬 THE GREAT HALL",
        "channels": [
            {"name": "general-chat", "topic": "Main community discussion for Mount & Blade II: Bannerlord & Realms of Dust."},
            {"name": "strategy-and-meta", "topic": "Starting realms, economics, troop progression, and conquest tactics."},
            {"name": "realm-heraldry", "topic": "Share your kingdom banners, custom clans, and faction screenshots."},
            {"name": "tavern-stories", "topic": "Campaign roleplay, glorious victories, and brutal defeats."},
        ]
    },
    {
        "category": "🛠️ FORGE & FEEDBACK",
        "channels": [
            {"name": "feedback-and-ideas", "topic": "Suggestions, balance feedback, and feature requests."},
            {"name": "bug-reports", "topic": "Report campaign bugs, crashes, and visual glitches."},
            {"name": "mod-troubleshooting", "topic": "Installation help, load order advice, and technical support."},
        ]
    },
    {
        "category": "🔒 WAR ROOM",
        "private": True,
        "channels": [
            {"name": "marshal-command", "topic": "Staff discussion and bot command center."},
            {"name": "vanguard-playtesting", "topic": "Pre-release testing, raw development builds, and private feedback."},
        ]
    }
]

WELCOME_EMBED = {
    "title": "⚔️ Welcome to Realms of Dust ⚔️",
    "description": (
        "**Realms of Dust** fractures Calradia into 52 balanced micro-realms on Day 1.\n"
        "Every ruler—including you—starts with **1 Town, 1 Castle, equal bound villages, and 0 troops**.\n\n"
        "### 📜 Rules of Engagement\n"
        "1. **Respect Your Fellow Warlords:** Maintain civil discourse in all channels.\n"
        "2. **Channel Discipline:** Keep bug reports in `#bug-reports` and balance discussion in `#feedback-and-ideas`.\n"
        "3. **No Spoilers / Cheats:** Keep discussion fair and authentic to the grind.\n\n"
        "### 🗺️ Useful Resources\n"
        "• Check <#announcements> for release links.\n"
        "• Check <#patch-notes> for current build details.\n"
        "• Join discussion in <#general-chat>!"
    )
}

@bot.event
async def on_ready():
    print(f"[Realms of Dust] Bot connected as {bot.user.name} ({bot.user.id})", flush=True)
    print(f"[Realms of Dust] Connected to {len(bot.guilds)} server(s):", flush=True)
    for g in bot.guilds:
        print(f" - {g.name} (ID: {g.id})", flush=True)
    
    try:
        synced = await bot.tree.sync()
        print(f"[Realms of Dust] Synced {len(synced)} application slash commands.", flush=True)
    except Exception as e:
        print(f"[Realms of Dust] Error syncing slash commands: {e}", flush=True)

async def execute_server_setup(guild, report_progress_func):
    """Core provisioning logic that creates all roles, categories, channels, and welcome post."""
    created_roles = {}

    await report_progress_func("⚔️ **Phase 1:** Creating server roles...")
    for role_cfg in ROLES_CONFIG:
        existing = discord.utils.get(guild.roles, name=role_cfg["name"])
        if existing:
            created_roles[role_cfg["name"]] = existing
        else:
            perms = discord.Permissions(administrator=True) if role_cfg.get("admin") else discord.Permissions.general()
            role = await guild.create_role(
                name=role_cfg["name"],
                colour=role_cfg["color"],
                hoist=role_cfg["hoist"],
                mentionable=role_cfg["mentionable"],
                permissions=perms
            )
            created_roles[role_cfg["name"]] = role

    marshal_role = created_roles.get("🛡️ Marshal")
    vanguard_role = created_roles.get("⚔️ Vanguard (Tester)")

    await report_progress_func("⚔️ **Phase 2:** Constructing categories and channels...")
    welcome_channel = None

    for cat_cfg in STRUCTURE:
        cat_name = cat_cfg["category"]
        existing_cat = discord.utils.get(guild.categories, name=cat_name)

        overwrites = {
            guild.default_role: discord.PermissionOverwrite(read_messages=True)
        }

        if cat_cfg.get("private"):
            overwrites[guild.default_role] = discord.PermissionOverwrite(read_messages=False)
            if marshal_role:
                overwrites[marshal_role] = discord.PermissionOverwrite(read_messages=True, send_messages=True)
            if vanguard_role:
                overwrites[vanguard_role] = discord.PermissionOverwrite(read_messages=True, send_messages=True)

        if not existing_cat:
            category = await guild.create_category(cat_name, overwrites=overwrites)
        else:
            category = existing_cat

        for chan_cfg in cat_cfg["channels"]:
            ch_name = chan_cfg["name"]
            existing_ch = discord.utils.get(category.text_channels, name=ch_name)

            chan_overwrites = dict(overwrites)
            if chan_cfg.get("read_only"):
                chan_overwrites[guild.default_role] = discord.PermissionOverwrite(
                    read_messages=True,
                    send_messages=False,
                    add_reactions=True
                )
                if marshal_role:
                    chan_overwrites[marshal_role] = discord.PermissionOverwrite(send_messages=True)

            if not existing_ch:
                channel = await guild.create_text_channel(
                    name=ch_name,
                    category=category,
                    topic=chan_cfg.get("topic", ""),
                    overwrites=chan_overwrites
                )
            else:
                channel = existing_ch

            if ch_name == "welcome-and-info":
                welcome_channel = channel

    if welcome_channel:
        history = [m async for m in welcome_channel.history(limit=5)]
        if not history:
            embed = discord.Embed(
                title=WELCOME_EMBED["title"],
                description=WELCOME_EMBED["description"],
                color=discord.Color.gold()
            )
            embed.set_footer(text="Realms of Dust • Calradia Shattered")
            await welcome_channel.send(embed=embed)

    await report_progress_func("✅ **Realms of Dust Server Provisioning Complete!** All roles, categories, and channels are ready.")

@bot.command(name="setup_server")
@commands.has_permissions(administrator=True)
async def setup_server_command(ctx):
    """Provisions all standard roles, categories, and channels for Realms of Dust via prefix command."""
    status_msg = await ctx.send("⚔️ **Realms of Dust Initializer:** Starting server provisioning...")

    async def report(msg):
        await status_msg.edit(content=msg)

    await execute_server_setup(ctx.guild, report)

@bot.tree.command(name="setup", description="Provision all standard Realms of Dust roles and channels.")
async def setup_slash(interaction: discord.Interaction):
    if not interaction.user.guild_permissions.administrator:
        await interaction.response.send_message("❌ Only server administrators can run this command.", ephemeral=True)
        return

    await interaction.response.send_message("⚔️ **Realms of Dust Initializer:** Starting server provisioning...")

    async def report(msg):
        await interaction.edit_original_response(content=msg)

    await execute_server_setup(interaction.guild, report)

@bot.tree.command(name="ping", description="Check bot latency.")
async def ping_slash(interaction: discord.Interaction):
    latency = round(bot.latency * 1000)
    await interaction.response.send_message(f"🏓 Pong! Latency is `{latency}ms`.")

@bot.tree.command(name="announce", description="Post an announcement embed to #announcements.")
async def announce_slash(interaction: discord.Interaction, title: str, message: str):
    if not interaction.user.guild_permissions.manage_guild:
        await interaction.response.send_message("❌ You do not have permission to post announcements.", ephemeral=True)
        return

    ann_channel = discord.utils.get(interaction.guild.text_channels, name="announcements")
    if not ann_channel:
        await interaction.response.send_message("❌ Could not find `#announcements` channel.", ephemeral=True)
        return

    embed = discord.Embed(title=title, description=message, color=discord.Color.gold())
    embed.set_author(name=interaction.user.display_name, icon_url=interaction.user.display_avatar.url)
    embed.set_footer(text="Realms of Dust • Announcement")
    
    await ann_channel.send(embed=embed)
    await interaction.response.send_message(f"✅ Announcement posted in {ann_channel.mention}!", ephemeral=True)

if __name__ == "__main__":
    bot.run(TOKEN)
