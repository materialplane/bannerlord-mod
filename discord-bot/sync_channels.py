import os
import sys
import asyncio
import discord
from dotenv import load_dotenv

if sys.platform == "win32":
    sys.stdout.reconfigure(encoding="utf-8")

load_dotenv()
TOKEN = os.getenv("DISCORD_BOT_TOKEN")

if not TOKEN:
    print("[Error] No token found")
    sys.exit(1)

intents = discord.Intents.default()
intents.guilds = True

client = discord.Client(intents=intents)

@client.event
async def on_ready():
    print(f"[Bot] Connected as {client.user}")
    for guild in client.guilds:
        print(f"\n--- Processing Guild: {guild.name} ({guild.id}) ---")

        # 1. Rename existing channels to preserve history
        feed_ch = discord.utils.get(guild.text_channels, name="feedback-and-ideas")
        if feed_ch:
            print("Renaming #feedback-and-ideas to #suggestions...")
            await feed_ch.edit(name="suggestions", topic="Suggestions, balance feedback, and feature requests.")

        bug_ch = discord.utils.get(guild.text_channels, name="bug-reports")
        if bug_ch:
            print("Renaming #bug-reports to #bugs...")
            await bug_ch.edit(name="bugs", topic="Report campaign bugs, crashes, and issues.")

        heraldry_ch = discord.utils.get(guild.text_channels, name="realm-heraldry")
        if heraldry_ch:
            print("Renaming #realm-heraldry to #screenshots-and-media...")
            await heraldry_ch.edit(name="screenshots-and-media", topic="Share screenshots, kingdom banners, clips, and battle media.")

        # 2. Delete obsolete channels
        channels_to_delete = [
            "strategy-and-meta",
            "tavern-stories",
            "mod-troubleshooting",
            "dev-log",
            "patch-notes",
            "marshal-command",
            "vanguard-playtesting"
        ]

        for ch_name in channels_to_delete:
            ch = discord.utils.get(guild.text_channels, name=ch_name)
            if ch:
                print(f"Deleting obsolete channel #{ch.name}...")
                await ch.delete(reason="Streamlining Discord channels")

        # 3. Delete obsolete categories
        war_room_cat = discord.utils.get(guild.categories, name="🔒 WAR ROOM")
        if war_room_cat:
            print("Deleting category 🔒 WAR ROOM...")
            await war_room_cat.delete(reason="Streamlining Discord channels")

        # 4. Ensure #dev-builds exists in NOTICE BOARD
        notice_cat = discord.utils.get(guild.categories, name="📢 WELCOME & NOTICE BOARD")
        dev_builds_ch = discord.utils.get(guild.text_channels, name="dev-builds")
        if not dev_builds_ch:
            print("Creating #dev-builds channel...")
            marshal_role = discord.utils.get(guild.roles, name="🛡️ Marshal")
            overwrites = {
                guild.default_role: discord.PermissionOverwrite(read_messages=True, send_messages=False, add_reactions=True)
            }
            if marshal_role:
                overwrites[marshal_role] = discord.PermissionOverwrite(send_messages=True)

            dev_builds_ch = await guild.create_text_channel(
                name="dev-builds",
                category=notice_cat,
                topic="Pre-release test builds, experimental patches, and ZIP updates.",
                overwrites=overwrites
            )
        elif notice_cat and dev_builds_ch.category != notice_cat:
            await dev_builds_ch.edit(category=notice_cat)

        # 5. Ensure #memes exists in THE GREAT HALL
        hall_cat = discord.utils.get(guild.categories, name="💬 THE GREAT HALL")
        memes_ch = discord.utils.get(guild.text_channels, name="memes")
        if not memes_ch:
            print("Creating #memes channel...")
            memes_ch = await guild.create_text_channel(
                name="memes",
                category=hall_cat,
                topic="Mount & Blade and Realms of Dust memes and humor."
            )
        elif hall_cat and memes_ch.category != hall_cat:
            await memes_ch.edit(category=hall_cat)

        # 6. Ensure #screenshots-and-media exists in THE GREAT HALL if not renamed from heraldry
        media_ch = discord.utils.get(guild.text_channels, name="screenshots-and-media")
        if not media_ch:
            print("Creating #screenshots-and-media channel...")
            media_ch = await guild.create_text_channel(
                name="screenshots-and-media",
                category=hall_cat,
                topic="Share screenshots, kingdom banners, clips, and battle media."
            )

        # 7. Update welcome message in #welcome-and-info
        welcome_ch = discord.utils.get(guild.text_channels, name="welcome-and-info")
        if welcome_ch:
            print("Updating #welcome-and-info overview message...")
            ann_ch = discord.utils.get(guild.text_channels, name="announcements")
            gen_ch = discord.utils.get(guild.text_channels, name="general-chat")
            sug_ch = discord.utils.get(guild.text_channels, name="suggestions")
            bugs_ch = discord.utils.get(guild.text_channels, name="bugs")

            desc = (
                "The Great Empires have fallen.\n\n"
                "Calradia lies fractured into 50+ rival micro-realms, each fighting for survival in the aftermath of total collapse. "
                "Every warlord starts from ground zero with an ancestral seat, bare walls, and a lone sword.\n\n"
                "Rebuild your realm, conquer the fractured lands, and forge a new empire from the dust.\n\n"
                "### 📜 Rules of Engagement\n"
                "1. **Respect Your Fellow Warlords:** Keep discussions civil.\n"
                f"2. **Channel Discipline:** Report issues in {bugs_ch.mention if bugs_ch else '#bugs'} "
                f"and balance suggestions in {sug_ch.mention if sug_ch else '#suggestions'}.\n"
                "3. **Fair Play:** Keep conquest authentic to the grind.\n\n"
                "### 🗺️ Channels Overview\n"
                f"• {ann_ch.mention if ann_ch else '#announcements'} — Releases, major updates, and patch notes\n"
                f"• {dev_builds_ch.mention if dev_builds_ch else '#dev-builds'} — Pre-release test builds and ZIP downloads\n"
                f"• {gen_ch.mention if gen_ch else '#general-chat'} — Main community discussion\n"
                f"• {media_ch.mention if media_ch else '#screenshots-and-media'} — Share your empire, banners, and clips\n"
                f"• {memes_ch.mention if memes_ch else '#memes'} — Memes and banter\n"
                f"• {sug_ch.mention if sug_ch else '#suggestions'} — Ideas and feedback\n"
                f"• {bugs_ch.mention if bugs_ch else '#bugs'} — Bug reports and crashes"
            )

            # Check if there is an existing bot message in welcome channel
            existing_msg = None
            async for m in welcome_ch.history(limit=5):
                if m.author == client.user:
                    existing_msg = m
                    break

            embed = discord.Embed(
                title="⚔️ Welcome to Realms of Dust ⚔️",
                description=desc,
                color=discord.Color.gold()
            )
            embed.set_footer(text="Realms of Dust • Calradia Shattered")

            if existing_msg:
                await existing_msg.edit(embed=embed)
                print("Edited existing welcome embed.")
            else:
                await welcome_ch.send(embed=embed)
                print("Posted fresh welcome embed.")

        print(f"\n--- Channel Streamlining Complete for {guild.name} ---")

    await client.close()

if __name__ == "__main__":
    client.run(TOKEN)
